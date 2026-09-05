using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Jobs;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly SsmsDbContext _dbContext;

    public JobService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static IQueryable<Job> BaseQuery(SsmsDbContext db) =>
        db.Jobs
            .Include(j => j.Client).ThenInclude(c => c.User)
            .Include(j => j.Category)
            .Include(j => j.AssignedWorker).ThenInclude(w => w!.User);

    private static JobResponse ToResponse(Job job) => new()
    {
        Id = job.Id,
        ClientId = job.ClientId,
        ClientName = job.Client.User.FullName,
        CategoryId = job.CategoryId,
        CategoryName = job.Category.Name,
        Title = job.Title,
        Description = job.Description,
        JobType = job.JobType,
        Location = job.Location,
        Budget = job.Budget,
        Status = job.Status,
        AssignedWorkerId = job.AssignedWorkerId,
        AssignedWorkerName = job.AssignedWorker?.User.FullName,
        CreatedAt = job.CreatedAt
    };

    public async Task<JobResponse?> GetByIdAsync(Guid id)
    {
        var job = await BaseQuery(_dbContext).FirstOrDefaultAsync(j => j.Id == id);
        return job is null ? null : ToResponse(job);
    }

    public async Task<IReadOnlyList<JobResponse>> GetAllAsync()
    {
        var jobs = await BaseQuery(_dbContext).OrderByDescending(j => j.CreatedAt).ToListAsync();
        return jobs.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<JobResponse>> GetOpenJobsAsync()
    {
        var jobs = await BaseQuery(_dbContext)
            .Where(j => j.Status == JobStatus.Open)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
        return jobs.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<JobResponse>> GetByClientIdAsync(Guid clientUserId)
    {
        var jobs = await BaseQuery(_dbContext)
            .Where(j => j.Client.UserId == clientUserId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
        return jobs.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<JobResponse>> GetByWorkerIdAsync(Guid workerUserId)
    {
        var jobs = await BaseQuery(_dbContext)
            .Where(j => j.AssignedWorker != null && j.AssignedWorker.UserId == workerUserId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
        return jobs.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<JobResponse>> SearchAsync(
        Guid? categoryId, string? location, decimal? minBudget, decimal? maxBudget)
    {
        var query = BaseQuery(_dbContext).Where(j => j.Status == JobStatus.Open);

        if (categoryId.HasValue)
            query = query.Where(j => j.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(j => j.Location != null && j.Location.Contains(location));

        if (minBudget.HasValue)
            query = query.Where(j => j.Budget >= minBudget.Value);

        if (maxBudget.HasValue)
            query = query.Where(j => j.Budget <= maxBudget.Value);

        var jobs = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
        return jobs.Select(ToResponse).ToList();
    }

    public async Task<JobResponse> CreateAsync(Guid clientUserId, CreateJobRequest request)
    {
        var clientProfile = await _dbContext.ClientProfiles
            .FirstOrDefaultAsync(c => c.UserId == clientUserId)
            ?? throw new InvalidOperationException("Client profile not found.");

        var job = new Job
        {
            ClientId = clientProfile.Id,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Description = request.Description,
            JobType = request.JobType,
            Location = request.Location,
            Budget = request.Budget,
            Status = JobStatus.Open
        };

        _dbContext.Jobs.Add(job);
        clientProfile.JobsPostedCount++;
        await _dbContext.SaveChangesAsync();

        var created = await BaseQuery(_dbContext).FirstAsync(j => j.Id == job.Id);
        return ToResponse(created);
    }

    public async Task<JobResponse?> UpdateAsync(Guid id, Guid clientUserId, UpdateJobRequest request)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null || job.Client.UserId != clientUserId)
            return null; // not found OR not the owner — same response either way, avoids leaking existence

        if (job.Status != JobStatus.Open)
            throw new InvalidOperationException("Only Open jobs can be edited.");

        job.CategoryId = request.CategoryId;
        job.Title = request.Title;
        job.Description = request.Description;
        job.JobType = request.JobType;
        job.Location = request.Location;
        job.Budget = request.Budget;
        job.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var updated = await BaseQuery(_dbContext).FirstAsync(j => j.Id == job.Id);
        return ToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid clientUserId)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null || job.Client.UserId != clientUserId)
            return false;

        job.IsDeleted = true; // soft delete — matches BaseEntity + global query filter
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AcceptApplicationAsync(Guid jobId, Guid applicationId, Guid clientUserId)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null || job.Client.UserId != clientUserId || job.Status != JobStatus.Open)
            return false;

        var acceptedApplication = job.Applications.FirstOrDefault(a => a.Id == applicationId);
        if (acceptedApplication is null)
            return false;

        acceptedApplication.Status = ApplicationStatus.Accepted;
        job.AssignedWorkerId = acceptedApplication.WorkerId;
        job.Status = JobStatus.Assigned;

        // Auto-reject every other pending application for this job.
        foreach (var otherApp in job.Applications.Where(a => a.Id != applicationId && a.Status == ApplicationStatus.Pending))
        {
            otherApp.Status = ApplicationStatus.Rejected;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> StartJobAsync(Guid jobId, Guid workerUserId)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job?.AssignedWorker is null || job.AssignedWorker.UserId != workerUserId || job.Status != JobStatus.Assigned)
            return false;

        job.Status = JobStatus.InProgress;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteJobAsync(Guid jobId, Guid workerUserId)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job?.AssignedWorker is null || job.AssignedWorker.UserId != workerUserId || job.Status != JobStatus.InProgress)
            return false;

        job.Status = JobStatus.Completed;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseJobAsync(Guid jobId, Guid clientUserId)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        // TODO: once Payment module exists, also require Payment.Status == Released
        // and block if there's an Open Dispute — flagged here so we don't forget.
        if (job is null || job.Client.UserId != clientUserId || job.Status != JobStatus.Completed)
            return false;

        job.Status = JobStatus.Closed;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelJobAsync(Guid jobId, Guid userId)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null) return false;

        var isOwner = job.Client.UserId == userId;
        var isAssignedWorker = job.AssignedWorker?.UserId == userId;
        if (!isOwner && !isAssignedWorker) return false;

        if (job.Status is JobStatus.Completed or JobStatus.Closed)
            throw new InvalidOperationException("Cannot cancel a completed or closed job.");

        job.Status = JobStatus.Cancelled;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}