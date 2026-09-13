using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Skills;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class WorkerSkillService : IWorkerSkillService
{
    private readonly SsmsDbContext _dbContext;

    public WorkerSkillService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static WorkerSkillResponse ToResponse(WorkerSkill ws) => new()
    {
        CategoryId = ws.CategoryId,
        CategoryName = ws.Category.Name
    };

    public async Task<IReadOnlyList<WorkerSkillResponse>> GetMySkillsAsync(Guid workerUserId)
    {
        var workerProfile = await _dbContext.WorkerProfiles
            .FirstOrDefaultAsync(w => w.UserId == workerUserId)
            ?? throw new InvalidOperationException("Worker profile not found.");

        return await GetForWorkerAsync(workerProfile.Id);
    }

    public async Task<IReadOnlyList<WorkerSkillResponse>> GetForWorkerAsync(Guid workerProfileId)
    {
        var skills = await _dbContext.WorkerSkills
            .Include(ws => ws.Category)
            .Where(ws => ws.WorkerProfileId == workerProfileId)
            .ToListAsync();

        return skills.Select(ToResponse).ToList();
    }

    public async Task<bool> AddSkillAsync(Guid workerUserId, AddSkillRequest request)
    {
        var workerProfile = await _dbContext.WorkerProfiles
            .FirstOrDefaultAsync(w => w.UserId == workerUserId)
            ?? throw new InvalidOperationException("Worker profile not found.");

        var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new InvalidOperationException("Category not found.");

        var alreadyHasSkill = await _dbContext.WorkerSkills
            .AnyAsync(ws => ws.WorkerProfileId == workerProfile.Id && ws.CategoryId == request.CategoryId);
        if (alreadyHasSkill)
            throw new InvalidOperationException("You already have this skill added.");

        _dbContext.WorkerSkills.Add(new WorkerSkill
        {
            WorkerProfileId = workerProfile.Id,
            CategoryId = request.CategoryId
        });

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveSkillAsync(Guid workerUserId, Guid categoryId)
    {
        var workerProfile = await _dbContext.WorkerProfiles
            .FirstOrDefaultAsync(w => w.UserId == workerUserId)
            ?? throw new InvalidOperationException("Worker profile not found.");

        var skill = await _dbContext.WorkerSkills
            .FirstOrDefaultAsync(ws => ws.WorkerProfileId == workerProfile.Id && ws.CategoryId == categoryId);

        if (skill is null) return false;

        _dbContext.WorkerSkills.Remove(skill);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}