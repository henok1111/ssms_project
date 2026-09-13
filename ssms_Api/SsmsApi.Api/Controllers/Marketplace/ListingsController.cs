using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/listings")]
[Authorize]
public class ListingsController : ControllerBase
{
    private readonly IListingService _listingService;

    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var listing = await _listingService.GetByIdAsync(id);
        return listing is null ? NotFound() : Ok(listing);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive()
    {
        var listings = await _listingService.GetActiveAsync();
        return Ok(listings);
    }
[HttpGet("search")]
[AllowAnonymous]
public async Task<IActionResult> Search(
    [FromQuery] Guid? categoryId,
    [FromQuery] string? keyword,
    [FromQuery] decimal? maxPrice,
    [FromQuery] string? location,
    [FromQuery] string? sortBy)
{
    var listings = await _listingService.SearchAsync(categoryId, keyword, maxPrice, location, sortBy);
    return Ok(listings);
}
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var listings = await _listingService.GetMyListingsAsync(CurrentUserId);
        return Ok(listings);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingRequest request)
    {
        var listing = await _listingService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListingRequest request)
    {
        var listing = await _listingService.UpdateAsync(id, CurrentUserId, request);
        return listing is null ? NotFound() : Ok(listing);
    }

    [HttpPost("{id:guid}/mark-sold")]
    public async Task<IActionResult> MarkAsSold(Guid id)
    {
        var success = await _listingService.MarkAsSoldAsync(id, CurrentUserId);
        return success ? Ok(new { message = "Listing marked as sold." }) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _listingService.DeleteAsync(id, CurrentUserId);
        return success ? NoContent() : NotFound();
    }
}