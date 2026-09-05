using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var payment = await _paymentService.GetForJobAsync(jobId);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpPost("initiate/{quoteId:guid}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Initiate(Guid quoteId)
    {
        var result = await _paymentService.InitiateAsync(quoteId, CurrentUserId);
        return Ok(result);
    }

    [HttpPost("confirm")]
    [AllowAnonymous] // Chapa's real webhook will call this later — no user is logged in during a webhook call
    public async Task<IActionResult> Confirm([FromQuery] string txRef)
    {
        var success = await _paymentService.ConfirmPaymentAsync(txRef);
        return success ? Ok(new { message = "Payment confirmed and held in escrow." }) : BadRequest();
    }

    [HttpPost("{paymentId:guid}/release")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Release(Guid paymentId)
    {
        var success = await _paymentService.ReleaseAsync(paymentId, CurrentUserId);
        return success ? Ok(new { message = "Payment released. Job closed." }) : BadRequest();
    }
}