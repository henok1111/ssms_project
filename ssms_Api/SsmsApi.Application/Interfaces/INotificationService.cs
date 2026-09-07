using SsmsApi.Application.DTOs.Notifications;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> GetForUserAsync(Guid userId);

    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);

    // Internal-use method — called by OTHER services (JobService, PaymentService, etc.),
    // never exposed directly as a public API endpoint.
    Task CreateAsync(Guid userId, NotificationType type, string content, Guid? relatedJobId = null);
}