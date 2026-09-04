using SsmsApi.Application.DTOs.Jobs;

namespace SsmsApi.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<JobResponse>> GetAllAsync();

    Task<IReadOnlyList<JobResponse>> GetOpenJobsAsync();

    Task<IReadOnlyList<JobResponse>> GetByClientIdAsync(
        Guid clientId);

    Task<IReadOnlyList<JobResponse>> SearchAsync(
        Guid? categoryId,
        string? location,
        decimal? minBudget,
        decimal? maxBudget);

Task<JobResponse> CreateAsync(Guid clientUserId, CreateJobRequest request);

  Task<JobResponse?> UpdateAsync(Guid id, Guid clientUserId, UpdateJobRequest request);
Task<bool> DeleteAsync(Guid id, Guid clientUserId);
 Task<bool> AcceptApplicationAsync(Guid jobId, Guid applicationId, Guid clientUserId);
   Task<bool> StartJobAsync(Guid jobId, Guid workerUserId);
Task<bool> CompleteJobAsync(Guid jobId, Guid workerUserId);
Task<bool> CloseJobAsync(Guid jobId, Guid clientUserId);
Task<bool> CancelJobAsync(Guid jobId, Guid userId); // service checks role + current status internally
}
