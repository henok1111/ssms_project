using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Notifications;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly SsmsDbContext _dbContext;
    private readonly IRealtimeNotifier _realtime;

    public NotificationService(SsmsDbContext dbContext, IRealtimeNotifier realtime)
    {
        _dbContext = dbContext;
        _realtime = realtime;
    }

    private static NotificationResponse ToResponse(Notification n) => new()
    {
        Id = n.Id,
        Type = n.Type,
        Content = n.Content,
        IsRead = n.IsRead,
        RelatedJobId = n.RelatedJobId,
        CreatedAt = n.CreatedAt
    };

    public async Task<IReadOnlyList<NotificationResponse>> GetForUserAsync(Guid userId)
    {
        var notifications = await _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(ToResponse).ToList();
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification is null) return false;

        notification.IsRead = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task CreateAsync(Guid userId, NotificationType type, string content, Guid? relatedJobId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Content = content,
            RelatedJobId = relatedJobId,
            IsRead = false
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        await _realtime.SendNotificationToUserAsync(userId, ToResponse(notification));
    }
}