using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Messages;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId:guid}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var messages = await _messageService.GetForJobAsync(jobId, CurrentUserId);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Send(Guid jobId, [FromBody] SendMessageRequest request)
    {
        var message = await _messageService.SendAsync(jobId, CurrentUserId, request);
        return Ok(message);
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid jobId)
    {
        var success = await _messageService.MarkAsReadAsync(jobId, CurrentUserId);
        return success ? Ok() : BadRequest();
    }
}