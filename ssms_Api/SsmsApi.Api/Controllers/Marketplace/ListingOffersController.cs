using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/listings/{listingId:guid}/offers")]
[Authorize]
public class ListingOffersController : ControllerBase
{
    private readonly IListingOfferService _offerService;

    public ListingOffersController(IListingOfferService offerService)
    {
        _offerService = offerService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetForListing(Guid listingId)
    {
        var offers = await _offerService.GetForListingAsync(listingId, CurrentUserId);
        return Ok(offers);
    }

    [HttpPost]
    public async Task<IActionResult> MakeOffer(Guid listingId, [FromBody] MakeOfferRequest request)
    {
        var offer = await _offerService.MakeOfferAsync(listingId, CurrentUserId, request);
        return Ok(offer);
    }

    [HttpPost("{offerId:guid}/accept")]
    public async Task<IActionResult> Accept(Guid listingId, Guid offerId)
    {
        var success = await _offerService.AcceptOfferAsync(offerId, CurrentUserId);
        return success ? Ok(new { message = "Offer accepted. Listing marked as sold." }) : BadRequest();
    }

    [HttpPost("{offerId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid listingId, Guid offerId)
    {
        var success = await _offerService.RejectOfferAsync(offerId, CurrentUserId);
        return success ? Ok(new { message = "Offer rejected." }) : BadRequest();
    }
}