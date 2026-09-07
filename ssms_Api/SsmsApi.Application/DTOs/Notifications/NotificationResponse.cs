using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Notifications;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public Guid? RelatedJobId { get; set; }
    public DateTime CreatedAt { get; set; }
}