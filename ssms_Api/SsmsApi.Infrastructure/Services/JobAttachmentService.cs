using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Attachments;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class JobAttachmentService : IJobAttachmentService
{
    private readonly SsmsDbContext _dbContext;
    private readonly IFileStorageService _fileStorage;

    private static readonly string[] AllowedContentTypes =
        { "image/jpeg", "image/png", "image/webp", "application/pdf" };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public JobAttachmentService(SsmsDbContext dbContext, IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    private static JobAttachmentResponse ToResponse(JobAttachment a) => new()
    {
        Id = a.Id,
        JobId = a.JobId,
        FileUrl = a.FileUrl,
        FileType = a.FileType,
        IsAiAnalyzed = a.IsAiAnalyzed,
        CreatedAt = a.CreatedAt
    };

    private static async Task<bool> IsJobParticipant(SsmsDbContext db, Guid jobId, Guid userId)
    {
        var job = await db.Jobs
            .Include(j => j.Client)
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null) return false;
        return job.Client.UserId == userId || job.AssignedWorker?.UserId == userId;
    }

    public async Task<IReadOnlyList<JobAttachmentResponse>> GetForJobAsync(Guid jobId)
    {
        var attachments = await _dbContext.JobAttachments
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return attachments.Select(ToResponse).ToList();
    }

    public async Task<JobAttachmentResponse> UploadAsync(
        Guid jobId, Guid userId, Stream fileStream, string fileName, string contentType)
    {
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new InvalidOperationException("Job not found.");

        if (!await IsJobParticipant(_dbContext, jobId, userId))
            throw new UnauthorizedAccessException("You are not a participant in this job.");

        if (!AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException("Only JPEG, PNG, WEBP images or PDF files are allowed.");

        if (fileStream.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("File size cannot exceed 5MB.");

        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        var attachment = new JobAttachment
        {
            JobId = jobId,
            FileUrl = fileUrl,
            FileType = contentType,
            IsAiAnalyzed = false
        };

        _dbContext.JobAttachments.Add(attachment);
        await _dbContext.SaveChangesAsync();

        return ToResponse(attachment);
    }

    public async Task<bool> DeleteAsync(Guid attachmentId, Guid userId)
    {
        var attachment = await _dbContext.JobAttachments
            .Include(a => a.Job).ThenInclude(j => j.Client)
            .Include(a => a.Job).ThenInclude(j => j.AssignedWorker)
            .FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (attachment is null) return false;

        var isParticipant = attachment.Job.Client.UserId == userId || attachment.Job.AssignedWorker?.UserId == userId;
        if (!isParticipant) return false;

        attachment.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}