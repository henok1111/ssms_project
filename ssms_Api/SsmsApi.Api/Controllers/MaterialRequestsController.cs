using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Materials;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId:guid}/material-requests")]
[Authorize]
public class MaterialRequestsController : ControllerBase
{
    private readonly IMaterialRequestService _materialRequestService;

    public MaterialRequestsController(IMaterialRequestService materialRequestService)
    {
        _materialRequestService = materialRequestService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var requests = await _materialRequestService.GetForJobAsync(jobId);
        return Ok(requests);
    }

    [HttpPost]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> Add(Guid jobId, [FromBody] AddMaterialRequestRequest request)
    {
        var result = await _materialRequestService.AddAsync(jobId, CurrentUserId, request);
        return Ok(result);
    }

    [HttpDelete("{requestId:guid}")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> Remove(Guid jobId, Guid requestId)
    {
        var success = await _materialRequestService.RemoveAsync(requestId, CurrentUserId);
        return success ? NoContent() : NotFound();
    }
}