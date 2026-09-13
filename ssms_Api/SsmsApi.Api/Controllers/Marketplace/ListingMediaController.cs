using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/listings/{listingId:guid}/media")]
[Authorize]
public class ListingMediaController : ControllerBase
{
    private readonly IListingMediaService _mediaService;

    public ListingMediaController(IListingMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Upload(Guid listingId, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        using var stream = file.OpenReadStream();
        var media = await _mediaService.UploadAsync(listingId, CurrentUserId, stream, file.FileName, file.ContentType);
        return Ok(media);
    }

    [HttpDelete("{mediaId:guid}")]
    public async Task<IActionResult> Delete(Guid listingId, Guid mediaId)
    {
        var success = await _mediaService.DeleteAsync(mediaId, CurrentUserId);
        return success ? NoContent() : NotFound();
    }
}