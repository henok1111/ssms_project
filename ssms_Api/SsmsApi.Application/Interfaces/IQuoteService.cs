using SsmsApi.Application.DTOs.Quotes;

namespace SsmsApi.Application.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponse?> GetForJobAsync(Guid jobId);

    Task<QuoteResponse> GenerateAsync(Guid jobId, Guid workerUserId, GenerateQuoteRequest request);

    Task<bool> ApproveAsync(Guid quoteId, Guid clientUserId);

    Task<bool> RejectAsync(Guid quoteId, Guid clientUserId);
}