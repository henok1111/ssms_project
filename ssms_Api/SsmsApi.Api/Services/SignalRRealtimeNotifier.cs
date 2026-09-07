using Microsoft.AspNetCore.SignalR;
using SsmsApi.Api.Hubs;
using SsmsApi.Application.DTOs.Messages;
using SsmsApi.Application.DTOs.Notifications;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Services;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRRealtimeNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToJobGroupAsync(Guid jobId, MessageResponse message)
    {
        await _hubContext.Clients.Group($"job-{jobId}").SendAsync("ReceiveMessage", message);
    }

    public async Task SendNotificationToUserAsync(Guid userId, NotificationResponse notification)
    {
        // SignalR's built-in User() targeting relies on the ClaimTypes.NameIdentifier
        // claim in the JWT — the same claim your controllers already read via CurrentUserId.
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
    }
}