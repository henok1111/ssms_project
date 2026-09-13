using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/listing-chat")]
[Authorize]
public class ListingChatController : ControllerBase
{
    private readonly IListingChatService _chatService;

    public ListingChatController(IListingChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("listings/{listingId:guid}/start")]
    public async Task<IActionResult> StartConversation(Guid listingId)
    {
        var conversation = await _chatService.StartOrGetConversationAsync(listingId, CurrentUserId);
        return Ok(conversation);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations()
    {
        var conversations = await _chatService.GetMyConversationsAsync(CurrentUserId);
        return Ok(conversations);
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid conversationId)
    {
        var messages = await _chatService.GetMessagesAsync(conversationId, CurrentUserId);
        return Ok(messages);
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendListingMessageRequest request)
    {
        var message = await _chatService.SendMessageAsync(conversationId, CurrentUserId, request);
        return Ok(message);
    }
}