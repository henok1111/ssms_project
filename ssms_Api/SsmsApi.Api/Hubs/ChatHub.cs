using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SsmsApi.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // Client calls this after connecting, to join the "room" for a specific Job.
  public async Task JoinListingConversation(string conversationId)
{
    await Groups.AddToGroupAsync(Context.ConnectionId, $"listing-conversation-{conversationId}");
}

public async Task LeaveListingConversation(string conversationId)
{
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"listing-conversation-{conversationId}");
}
}