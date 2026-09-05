using SsmsApi.Application.DTOs.Payments;

namespace SsmsApi.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse?> GetForJobAsync(Guid jobId);

    Task<InitiatePaymentResponse> InitiateAsync(Guid quoteId, Guid clientUserId);

    Task<bool> ConfirmPaymentAsync(string txRef);

    Task<bool> ReleaseAsync(Guid paymentId, Guid clientUserId);
}