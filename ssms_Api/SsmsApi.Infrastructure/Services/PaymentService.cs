using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Payments;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly SsmsDbContext _dbContext;
    private readonly IPaymentGatewayService _gateway;

    private const decimal CommissionRate = 0.10m; // 10% platform commission — adjust as needed

    public PaymentService(SsmsDbContext dbContext, IPaymentGatewayService gateway)
    {
        _dbContext = dbContext;
        _gateway = gateway;
    }

    private static PaymentResponse ToResponse(Payment p) => new()
    {
        Id = p.Id,
        JobId = p.JobId,
        Amount = p.Amount,
        PlatformCommission = p.PlatformCommission,
        AmountReleasedToWorker = p.AmountReleasedToWorker,
        Status = p.Status,
        TxRef = p.TxRef,
        PaidAt = p.PaidAt,
        ReleasedAt = p.ReleasedAt
    };

    public async Task<PaymentResponse?> GetForJobAsync(Guid jobId)
    {
        var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.JobId == jobId);
        return payment is null ? null : ToResponse(payment);
    }

    public async Task<InitiatePaymentResponse> InitiateAsync(Guid quoteId, Guid clientUserId)
    {
        var quote = await _dbContext.Quotes
            .Include(q => q.Job).ThenInclude(j => j.Client)
            .FirstOrDefaultAsync(q => q.Id == quoteId)
            ?? throw new InvalidOperationException("Quote not found.");

        if (quote.Job.Client.UserId != clientUserId)
            throw new UnauthorizedAccessException("Only the job's client can initiate payment.");

        if (quote.Status != QuoteStatus.Approved)
            throw new InvalidOperationException("Quote must be approved before payment can be initiated.");

        var existingPayment = await _dbContext.Payments.AnyAsync(p => p.QuoteId == quoteId);
        if (existingPayment)
            throw new InvalidOperationException("A payment already exists for this quote.");

        var txRef = $"ssms-{Guid.NewGuid():N}";
        var commission = quote.TotalCost * CommissionRate;

        var gatewayResult = await _gateway.InitiatePaymentAsync(quote.TotalCost, "ETB", txRef);

        var payment = new Payment
        {
            JobId = quote.JobId,
            QuoteId = quote.Id,
            Amount = quote.TotalCost,
            PlatformCommission = commission,
            Status = PaymentStatus.Pending,
            GatewayProvider = "Chapa",
            TxRef = txRef,
            CheckoutUrl = gatewayResult.CheckoutUrl
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        return new InitiatePaymentResponse
        {
            PaymentId = payment.Id,
            CheckoutUrl = gatewayResult.CheckoutUrl,
            TxRef = txRef,
            Amount = quote.TotalCost
        };
    }

    public async Task<bool> ConfirmPaymentAsync(string txRef)
    {
        var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.TxRef == txRef);
        if (payment is null) return false;

        var verification = await _gateway.VerifyPaymentAsync(txRef);
        if (!verification.Success || verification.Status != "success")
            return false;

        payment.Status = PaymentStatus.Held;
        payment.PaidAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReleaseAsync(Guid paymentId, Guid clientUserId)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.Job).ThenInclude(j => j.Client)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment is null || payment.Job.Client.UserId != clientUserId)
            return false;

        if (payment.Status != PaymentStatus.Held)
            throw new InvalidOperationException("Payment must be Held before it can be released.");

        if (payment.Job.Status != JobStatus.Completed)
            throw new InvalidOperationException("Job must be marked Completed before releasing payment.");

        var openDispute = await _dbContext.Disputes
            .AnyAsync(d => d.JobId == payment.JobId && d.Status != DisputeStatus.Resolved && d.Status != DisputeStatus.Rejected);
        if (openDispute)
            throw new InvalidOperationException("Cannot release payment while a dispute is open.");

        payment.Status = PaymentStatus.Released;
        payment.ReleasedAt = DateTime.UtcNow;
        payment.Job.Status = JobStatus.Closed;

        await _dbContext.SaveChangesAsync();
        return true;
    }
}