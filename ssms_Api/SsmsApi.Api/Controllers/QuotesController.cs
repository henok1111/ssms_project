using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Quotes;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId:guid}/quote")]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get(Guid jobId)
    {
        var quote = await _quoteService.GetForJobAsync(jobId);
        return quote is null ? NotFound() : Ok(quote);
    }

    [HttpPost]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> Generate(Guid jobId, [FromBody] GenerateQuoteRequest request)
    {
        var quote = await _quoteService.GenerateAsync(jobId, CurrentUserId, request);
        return Ok(quote);
    }

    [HttpPost("approve")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Approve(Guid jobId, [FromQuery] Guid quoteId)
    {
        var success = await _quoteService.ApproveAsync(quoteId, CurrentUserId);
        return success ? Ok(new { message = "Quote approved. Material orders placed." }) : BadRequest();
    }

    [HttpPost("reject")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Reject(Guid jobId, [FromQuery] Guid quoteId)
    {
        var success = await _quoteService.RejectAsync(quoteId, CurrentUserId);
        return success ? Ok(new { message = "Quote rejected." }) : BadRequest();
    }
}