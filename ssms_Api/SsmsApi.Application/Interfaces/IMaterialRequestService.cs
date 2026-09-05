using SsmsApi.Application.DTOs.Materials;

namespace SsmsApi.Application.Interfaces;

public interface IMaterialRequestService
{
    Task<IReadOnlyList<MaterialRequestResponse>> GetForJobAsync(Guid jobId);

    Task<MaterialRequestResponse> AddAsync(Guid jobId, Guid workerUserId, AddMaterialRequestRequest request);

    Task<bool> RemoveAsync(Guid requestId, Guid workerUserId);
}