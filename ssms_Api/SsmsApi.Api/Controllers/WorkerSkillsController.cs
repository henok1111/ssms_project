using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Skills;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/worker-skills")]
[Authorize]
public class WorkerSkillsController : ControllerBase
{
    private readonly IWorkerSkillService _skillService;

    public WorkerSkillsController(IWorkerSkillService skillService)
    {
        _skillService = skillService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("mine")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> GetMine()
    {
        var skills = await _skillService.GetMySkillsAsync(CurrentUserId);
        return Ok(skills);
    }

    [HttpGet("worker/{workerProfileId:guid}")]
    public async Task<IActionResult> GetForWorker(Guid workerProfileId)
    {
        var skills = await _skillService.GetForWorkerAsync(workerProfileId);
        return Ok(skills);
    }

    [HttpPost]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> AddSkill([FromBody] AddSkillRequest request)
    {
        var success = await _skillService.AddSkillAsync(CurrentUserId, request);
        return success ? Ok(new { message = "Skill added." }) : BadRequest();
    }

    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> RemoveSkill(Guid categoryId)
    {
        var success = await _skillService.RemoveSkillAsync(CurrentUserId, categoryId);
        return success ? NoContent() : NotFound();
    }
}