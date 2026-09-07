using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId:guid}/attachments")]
[Authorize]
public class JobAttachmentsController : ControllerBase
{
    private readonly IJobAttachmentService _attachmentService;

    public JobAttachmentsController(IJobAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetForJob(Guid jobId)
    {
        var attachments = await _attachmentService.GetForJobAsync(jobId);
        return Ok(attachments);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(Guid jobId, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        using var stream = file.OpenReadStream();
        var attachment = await _attachmentService.UploadAsync(
            jobId, CurrentUserId, stream, file.FileName, file.ContentType);

        return Ok(attachment);
    }

    [HttpDelete("{attachmentId:guid}")]
    public async Task<IActionResult> Delete(Guid jobId, Guid attachmentId)
    {
        var success = await _attachmentService.DeleteAsync(attachmentId, CurrentUserId);
        return success ? NoContent() : NotFound();
    }
}