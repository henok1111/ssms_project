using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var notifications = await _notificationService.GetForUserAsync(CurrentUserId);
        return Ok(notifications);
    }

    [HttpPost("{id:guid}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var success = await _notificationService.MarkAsReadAsync(id, CurrentUserId);
        return success ? Ok() : NotFound();
    }
}