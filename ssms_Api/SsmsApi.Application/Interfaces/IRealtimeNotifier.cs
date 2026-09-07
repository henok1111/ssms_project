using SsmsApi.Application.DTOs.Messages;
using SsmsApi.Application.DTOs.Notifications;

namespace SsmsApi.Application.Interfaces;

public interface IRealtimeNotifier
{
    Task SendMessageToJobGroupAsync(Guid jobId, MessageResponse message);

    Task SendNotificationToUserAsync(Guid userId, NotificationResponse notification);
}