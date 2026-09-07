using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Disputes;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/disputes")]
[Authorize]
public class DisputesController : ControllerBase
{
    private readonly IDisputeService _disputeService;

    public DisputesController(IDisputeService disputeService)
    {
        _disputeService = disputeService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var disputes = await _disputeService.GetForJobAsync(jobId);
        return Ok(disputes);
    }

    [HttpGet("open")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOpen()
    {
        var disputes = await _disputeService.GetAllOpenAsync();
        return Ok(disputes);
    }

    [HttpPost]
    public async Task<IActionResult> Raise([FromBody] RaiseDisputeRequest request)
    {
        var dispute = await _disputeService.RaiseAsync(request.JobId, CurrentUserId, request);
        return Ok(dispute);
    }

    [HttpPost("{id:guid}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveDisputeRequest request)
    {
        var success = await _disputeService.ResolveAsync(id, request);
        return success ? Ok(new { message = "Dispute resolved." }) : NotFound();
    }
}