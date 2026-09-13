using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/saved-listings")]
[Authorize]
public class SavedListingsController : ControllerBase
{
    private readonly ISavedListingService _savedListingService;

    public SavedListingsController(ISavedListingService savedListingService)
    {
        _savedListingService = savedListingService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var saved = await _savedListingService.GetMySavedAsync(CurrentUserId);
        return Ok(saved);
    }

    [HttpPost("{listingId:guid}")]
    public async Task<IActionResult> Save(Guid listingId)
    {
        var success = await _savedListingService.SaveAsync(CurrentUserId, listingId);
        return success ? Ok(new { message = "Listing saved." }) : NotFound();
    }

    [HttpDelete("{listingId:guid}")]
    public async Task<IActionResult> Unsave(Guid listingId)
    {
        var success = await _savedListingService.UnsaveAsync(CurrentUserId, listingId);
        return success ? NoContent() : NotFound();
    }
}