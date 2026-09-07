using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Messages;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly SsmsDbContext _dbContext;
    private readonly IRealtimeNotifier _realtime;

    public MessageService(SsmsDbContext dbContext, IRealtimeNotifier realtime)
    {
        _dbContext = dbContext;
        _realtime = realtime;
    }

    private static MessageResponse ToResponse(Message m) => new()
    {
        Id = m.Id,
        JobId = m.JobId,
        SenderId = m.SenderId,
        SenderName = m.Sender.FullName,
        Content = m.Content,
        IsRead = m.IsRead,
        CreatedAt = m.CreatedAt
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

    public async Task<IReadOnlyList<MessageResponse>> GetForJobAsync(Guid jobId, Guid userId)
    {
        if (!await IsJobParticipant(_dbContext, jobId, userId))
            throw new UnauthorizedAccessException("You are not a participant in this job.");

        var messages = await _dbContext.Messages
            .Include(m => m.Sender)
            .Where(m => m.JobId == jobId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(ToResponse).ToList();
    }

    public async Task<MessageResponse> SendAsync(Guid jobId, Guid senderId, SendMessageRequest request)
    {
        if (!await IsJobParticipant(_dbContext, jobId, senderId))
            throw new UnauthorizedAccessException("You are not a participant in this job.");
 if (ContainsContactInfo(request.Content))
        throw new InvalidOperationException("Messages cannot contain phone numbers, emails, or requests to communicate outside the platform.");
        var message = new Message
        {
            JobId = jobId,
            SenderId = senderId,
            Content = request.Content,
            IsRead = false
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        message.Sender = (await _dbContext.Users.FindAsync(senderId))!;
        var response = ToResponse(message);

        // Push it live to anyone currently viewing this job's chat.
        await _realtime.SendMessageToJobGroupAsync(jobId, response);

        return response;
    }

    public async Task<bool> MarkAsReadAsync(Guid jobId, Guid userId)
    {
        if (!await IsJobParticipant(_dbContext, jobId, userId))
            return false;

        var unread = await _dbContext.Messages
            .Where(m => m.JobId == jobId && m.SenderId != userId && !m.IsRead)
            .ToListAsync();

        foreach (var m in unread) m.IsRead = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static bool ContainsContactInfo(string content)
{
    // Matches common Ethiopian and international phone number patterns:
    // 09xxxxxxxx, 07xxxxxxxx, +2519xxxxxxxx, or any 8+ digit run.
    var phonePattern = @"(\+?251|0)?[97]\d{8}|\d{8,}";

    // Matches email addresses.
    var emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";

    // Common phrases people use to redirect off-platform.
    var suspiciousPhrases = new[]
    {
        "telegram", "whatsapp", "call me", "text me", "my number",
        "cash only", "pay direct", "outside the app", "off the app"
    };

    if (System.Text.RegularExpressions.Regex.IsMatch(content, phonePattern))
        return true;

    if (System.Text.RegularExpressions.Regex.IsMatch(content, emailPattern))
        return true;

    var lowerContent = content.ToLowerInvariant();
    return suspiciousPhrases.Any(phrase => lowerContent.Contains(phrase));
}
}