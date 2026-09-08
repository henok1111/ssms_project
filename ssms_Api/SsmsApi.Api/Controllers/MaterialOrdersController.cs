using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/material-orders")]
[Authorize]
public class MaterialOrdersController : ControllerBase
{
    private readonly IMaterialOrderService _orderService;

    public MaterialOrdersController(IMaterialOrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("mine")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> GetMine()
    {
        var orders = await _orderService.GetForSupplierAsync(CurrentUserId);
        return Ok(orders);
    }

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var orders = await _orderService.GetForJobAsync(jobId);
        return Ok(orders);
    }

    [HttpPost("{id:guid}/confirm")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var success = await _orderService.ConfirmAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Order confirmed." }) : NotFound();
    }

    [HttpPost("{id:guid}/fulfill")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Fulfill(Guid id)
    {
        var success = await _orderService.FulfillAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Order marked as fulfilled." }) : NotFound();
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var success = await _orderService.CancelAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Order cancelled, stock restored." }) : NotFound();
    }
}