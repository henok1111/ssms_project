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

    Task<JobResponse> CreateAsync(
        CreateJobRequest request);

    Task<JobResponse?> UpdateAsync(
        Guid id,
        UpdateJobRequest request);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> AssignWorkerAsync(
        Guid jobId,
        Guid workerId);

    Task<bool> StartJobAsync(
        Guid jobId);

    Task<bool> CompleteJobAsync(
        Guid jobId);

    Task<bool> CloseJobAsync(
        Guid jobId);

    Task<bool> CancelJobAsync(
        Guid jobId);
}
