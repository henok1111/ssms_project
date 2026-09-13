namespace SsmsApi.Application.Interfaces;

public interface IRealtimeNotifier
{
    Task SendToGroupAsync(string groupName, string eventName, object payload);

    Task SendNotificationToUserAsync(Guid userId, object notification);
}