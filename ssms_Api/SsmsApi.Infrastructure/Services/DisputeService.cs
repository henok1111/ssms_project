using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Disputes;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class DisputeService : IDisputeService
{
    private readonly SsmsDbContext _dbContext;
private readonly INotificationService _notificationService;

public DisputeService(SsmsDbContext dbContext, INotificationService notificationService)
{
    _dbContext = dbContext;
    _notificationService = notificationService;
}
    private static DisputeResponse ToResponse(Dispute d) => new()
    {
        Id = d.Id,
        JobId = d.JobId,
        RaisedById = d.RaisedById,
        RaisedByName = d.RaisedBy.FullName,
        Reason = d.Reason,
        Status = d.Status,
        AdminResolutionNote = d.AdminResolutionNote,
        CreatedAt = d.CreatedAt
    };

    public async Task<IReadOnlyList<DisputeResponse>> GetAllOpenAsync()
    {
        var disputes = await _dbContext.Disputes
            .Include(d => d.RaisedBy)
            .Where(d => d.Status == DisputeStatus.Open || d.Status == DisputeStatus.UnderReview)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync();

        return disputes.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<DisputeResponse>> GetForJobAsync(Guid jobId)
    {
        var disputes = await _dbContext.Disputes
            .Include(d => d.RaisedBy)
            .Where(d => d.JobId == jobId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        return disputes.Select(ToResponse).ToList();
    }

    public async Task<DisputeResponse> RaiseAsync(Guid jobId, Guid userId, RaiseDisputeRequest request)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.Client)
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new InvalidOperationException("Job not found.");

        var isParticipant = job.Client.UserId == userId || job.AssignedWorker?.UserId == userId;
        if (!isParticipant)
            throw new UnauthorizedAccessException("You are not a participant in this job.");

        var alreadyOpen = await _dbContext.Disputes
            .AnyAsync(d => d.JobId == jobId && (d.Status == DisputeStatus.Open || d.Status == DisputeStatus.UnderReview));
        if (alreadyOpen)
            throw new InvalidOperationException("An open dispute already exists for this job.");

        var dispute = new Dispute
        {
            JobId = jobId,
            RaisedById = userId,
            Reason = request.Reason,
            Status = DisputeStatus.Open
        };

        _dbContext.Disputes.Add(dispute);
        await _dbContext.SaveChangesAsync();

        dispute.RaisedBy = (await _dbContext.Users.FindAsync(userId))!;
        // Notify whichever participant DIDN'T raise the dispute.
var otherPartyUserId = job.Client.UserId == userId
    ? job.AssignedWorker?.UserId
    : job.Client.UserId;

if (otherPartyUserId.HasValue)
{
    await _notificationService.CreateAsync(
        otherPartyUserId.Value,
        NotificationType.OrderStatusUpdate, // no dedicated "Dispute" type yet — reuse closest existing one for now
        $"A dispute was raised on job \"{job.Title}\". An admin will review it.",
        jobId
    );
}
        return ToResponse(dispute);
    }

    public async Task<bool> ResolveAsync(Guid disputeId, ResolveDisputeRequest request)
    {
        var dispute = await _dbContext.Disputes.FirstOrDefaultAsync(d => d.Id == disputeId);
        if (dispute is null) return false;

        dispute.Status = request.Approve ? DisputeStatus.Resolved : DisputeStatus.Rejected;
        dispute.AdminResolutionNote = request.AdminResolutionNote;

        await _dbContext.SaveChangesAsync();
        return true;
    }
}