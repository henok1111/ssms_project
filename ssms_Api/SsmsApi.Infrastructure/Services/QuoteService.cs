using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Quotes;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class QuoteService : IQuoteService
{
    private readonly SsmsDbContext _dbContext;

    public QuoteService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static QuoteResponse ToResponse(Quote q) => new()
    {
        Id = q.Id,
        JobId = q.JobId,
        LaborCost = q.LaborCost,
        MaterialsCost = q.MaterialsCost,
        Status = q.Status,
        CreatedAt = q.CreatedAt
    };

    public async Task<QuoteResponse?> GetForJobAsync(Guid jobId)
    {
        var quote = await _dbContext.Quotes.FirstOrDefaultAsync(q => q.JobId == jobId);
        return quote is null ? null : ToResponse(quote);
    }

    public async Task<QuoteResponse> GenerateAsync(Guid jobId, Guid workerUserId, GenerateQuoteRequest request)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new InvalidOperationException("Job not found.");

        if (job.AssignedWorker is null || job.AssignedWorker.UserId != workerUserId)
            throw new UnauthorizedAccessException("Only the assigned worker can generate a quote.");

        var alreadyExists = await _dbContext.Quotes.AnyAsync(q => q.JobId == jobId);
        if (alreadyExists)
            throw new InvalidOperationException("A quote already exists for this job.");

        // MaterialsCost is derived, not typed in manually — this is the
        // whole point of snapshotting UnitPriceAtRequest earlier: the
        // materials total is always trustworthy, computed from real data.
        var materialsCost = await _dbContext.JobMaterialRequests
            .Where(r => r.JobId == jobId)
            .SumAsync(r => r.QuantityNeeded * r.UnitPriceAtRequest);

        var quote = new Quote
        {
            JobId = jobId,
            LaborCost = request.LaborCost,
            MaterialsCost = materialsCost,
            Status = QuoteStatus.PendingApproval
        };

        _dbContext.Quotes.Add(quote);
        await _dbContext.SaveChangesAsync();

        return ToResponse(quote);
    }

    public async Task<bool> ApproveAsync(Guid quoteId, Guid clientUserId)
    {
        var quote = await _dbContext.Quotes
            .Include(q => q.Job).ThenInclude(j => j.Client)
            .FirstOrDefaultAsync(q => q.Id == quoteId);

        if (quote is null || quote.Job.Client.UserId != clientUserId || quote.Status != QuoteStatus.PendingApproval)
            return false;

        quote.Status = QuoteStatus.Approved;

        // Approving the Quote is what actually triggers real MaterialOrders —
        // this is the moment the "combined quote" concept from our very first
        // design conversation becomes real: approval commits to buying materials.
        var materialRequests = await _dbContext.JobMaterialRequests
            .Where(r => r.JobId == quote.JobId)
            .Include(r => r.MaterialItem)
            .ToListAsync();

        foreach (var mr in materialRequests)
        {
            _dbContext.MaterialOrders.Add(new MaterialOrder
            {
                JobMaterialRequestId = mr.Id,
                SupplierId = mr.MaterialItem.SupplierId,
                QuantityOrdered = mr.QuantityNeeded,
                TotalPrice = mr.QuantityNeeded * mr.UnitPriceAtRequest,
                Status = Domain.Enums.OrderStatus.Pending
            });

            // Deduct stock now that the order is committed.
            mr.MaterialItem.StockQuantity -= mr.QuantityNeeded;
        }

        // TODO: once Payment module is built, create the Payment record here
        // (Status: Pending -> Held via the fake Chapa gateway) tied to this Quote.

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid quoteId, Guid clientUserId)
    {
        var quote = await _dbContext.Quotes
            .Include(q => q.Job).ThenInclude(j => j.Client)
            .FirstOrDefaultAsync(q => q.Id == quoteId);

        if (quote is null || quote.Job.Client.UserId != clientUserId || quote.Status != QuoteStatus.PendingApproval)
            return false;

        quote.Status = QuoteStatus.Rejected;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}