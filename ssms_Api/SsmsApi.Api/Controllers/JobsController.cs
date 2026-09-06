using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Jobs;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // every endpoint here requires a logged-in user by default
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{id:guid}")]
    [AllowAnonymous] // this endpoint is public
    public async Task<IActionResult> GetById(Guid id)
    {
        var job = await _jobService.GetByIdAsync(id);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpGet("open")]
    [AllowAnonymous] // this endpoint is public
    public async Task<IActionResult> GetOpenJobs()
    {
        var jobs = await _jobService.GetOpenJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("search")]
    [AllowAnonymous] // this endpoint is public
    public async Task<IActionResult> Search(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? location,
        [FromQuery] decimal? minBudget,
        [FromQuery] decimal? maxBudget)
    {
        var jobs = await _jobService.SearchAsync(categoryId, location, minBudget, maxBudget);
        return Ok(jobs);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetMyJobs()
    {
        var jobs = await _jobService.GetByClientIdAsync(CurrentUserId);
        return Ok(jobs);
    }

    [HttpGet("assigned-to-me")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> GetMyAssignedJobs()
    {
        var jobs = await _jobService.GetByWorkerIdAsync(CurrentUserId);
        return Ok(jobs);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _jobService.GetAllAsync();
        return Ok(jobs);
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Create([FromBody] CreateJobRequest request)
    {
        var job = await _jobService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobRequest request)
    {
        var job = await _jobService.UpdateAsync(id, CurrentUserId, request);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _jobService.DeleteAsync(id, CurrentUserId);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/applications/{applicationId:guid}/accept")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> AcceptApplication(Guid id, Guid applicationId)
    {
        var success = await _jobService.AcceptApplicationAsync(id, applicationId, CurrentUserId);
        return success ? Ok(new { message = "Application accepted." }) : BadRequest(new { message = "Unable to accept this application." });
    }

    [HttpPost("{id:guid}/start")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> Start(Guid id)
    {
        var success = await _jobService.StartJobAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Job started." }) : BadRequest(new { message = "Unable to start this job." });
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var success = await _jobService.CompleteJobAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Job marked complete." }) : BadRequest(new { message = "Unable to complete this job." });
    }

   

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var success = await _jobService.CancelJobAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Job cancelled." }) : BadRequest(new { message = "Unable to cancel this job." });
    }
   [HttpPost("{id:guid}/apply")]
[Authorize(Roles = "Worker")]
public async Task<IActionResult> Apply(Guid id, [FromBody] CreateJobApplicationRequest request)
{
    try
    {
        var application = await _jobService.ApplyAsync(id, CurrentUserId, request);
        return Ok(application);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }

}
[HttpGet("{id:guid}/applications")]
[Authorize(Roles = "Client")]
public async Task<IActionResult> GetApplications(Guid id)
{
    try
    {
        var applications = await _jobService.GetApplicationsForJobAsync(id, CurrentUserId);
        return Ok(applications);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
}
}