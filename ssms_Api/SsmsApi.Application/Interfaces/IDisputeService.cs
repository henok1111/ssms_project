using SsmsApi.Application.DTOs.Disputes;

namespace SsmsApi.Application.Interfaces;

public interface IDisputeService
{
    Task<IReadOnlyList<DisputeResponse>> GetAllOpenAsync();

    Task<IReadOnlyList<DisputeResponse>> GetForJobAsync(Guid jobId);

    Task<DisputeResponse> RaiseAsync(Guid jobId, Guid userId, RaiseDisputeRequest request);

    Task<bool> ResolveAsync(Guid disputeId, ResolveDisputeRequest request);
}