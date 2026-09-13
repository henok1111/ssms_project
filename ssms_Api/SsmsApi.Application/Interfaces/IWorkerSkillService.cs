using SsmsApi.Application.DTOs.Skills;

namespace SsmsApi.Application.Interfaces;

public interface IWorkerSkillService
{
    Task<IReadOnlyList<WorkerSkillResponse>> GetMySkillsAsync(Guid workerUserId);

    Task<IReadOnlyList<WorkerSkillResponse>> GetForWorkerAsync(Guid workerProfileId);

    Task<bool> AddSkillAsync(Guid workerUserId, AddSkillRequest request);

    Task<bool> RemoveSkillAsync(Guid workerUserId, Guid categoryId);
}