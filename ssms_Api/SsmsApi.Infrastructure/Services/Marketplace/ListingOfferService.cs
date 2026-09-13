using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Domain.Enums.Marketplace;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services.Marketplace;

public class ListingOfferService : IListingOfferService
{
    private readonly SsmsDbContext _dbContext;

    public ListingOfferService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static ListingOfferResponse ToResponse(ListingOffer o) => new()
    {
        Id = o.Id,
        ListingId = o.ListingId,
        BuyerId = o.BuyerId,
        BuyerName = o.Buyer.FullName,
        OfferedPrice = o.OfferedPrice,
        Message = o.Message,
        Status = o.Status,
        CreatedAt = o.CreatedAt
    };

    public async Task<IReadOnlyList<ListingOfferResponse>> GetForListingAsync(Guid listingId, Guid sellerUserId)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null || listing.SellerId != sellerUserId)
            return new List<ListingOfferResponse>();

        // Sorted highest offer first — exactly the "compare prices" feature:
        // the seller sees the best offers at the top without manual sorting.
        var offers = await _dbContext.ListingOffers
            .Include(o => o.Buyer)
            .Where(o => o.ListingId == listingId)
            .OrderByDescending(o => o.OfferedPrice)
            .ToListAsync();

        return offers.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<ListingOfferResponse>> GetMyOffersAsync(Guid buyerUserId)
    {
        var offers = await _dbContext.ListingOffers
            .Include(o => o.Buyer)
            .Where(o => o.BuyerId == buyerUserId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return offers.Select(ToResponse).ToList();
    }

    public async Task<ListingOfferResponse> MakeOfferAsync(Guid listingId, Guid buyerUserId, MakeOfferRequest request)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == listingId)
            ?? throw new InvalidOperationException("Listing not found.");

        if (listing.Status != ListingStatus.Active)
            throw new InvalidOperationException("This listing is no longer available.");

        if (listing.SellerId == buyerUserId)
            throw new InvalidOperationException("You cannot make an offer on your own listing.");

        if (request.OfferedPrice <= 0)
            throw new InvalidOperationException("Offered price must be greater than zero.");

        // Only one active (Pending) offer per buyer per listing — prevents spam,
        // encourages updating an existing offer rather than flooding with new ones.
        var existingPending = await _dbContext.ListingOffers
            .AnyAsync(o => o.ListingId == listingId && o.BuyerId == buyerUserId && o.Status == OfferStatus.Pending);
        if (existingPending)
            throw new InvalidOperationException("You already have a pending offer on this listing.");

        var offer = new ListingOffer
        {
            ListingId = listingId,
            BuyerId = buyerUserId,
            OfferedPrice = request.OfferedPrice,
            Message = request.Message,
            Status = OfferStatus.Pending
        };

        _dbContext.ListingOffers.Add(offer);
        await _dbContext.SaveChangesAsync();

        offer.Buyer = (await _dbContext.Users.FindAsync(buyerUserId))!;
        return ToResponse(offer);
    }

    public async Task<bool> AcceptOfferAsync(Guid offerId, Guid sellerUserId)
    {
        var offer = await _dbContext.ListingOffers
            .Include(o => o.Listing)
            .FirstOrDefaultAsync(o => o.Id == offerId);

        if (offer is null || offer.Listing.SellerId != sellerUserId || offer.Status != OfferStatus.Pending)
            return false;

        offer.Status = OfferStatus.Accepted;
        offer.Listing.Status = ListingStatus.Sold;

        // Auto-reject every other pending offer once one is accepted —
        // same pattern as JobService.AcceptApplicationAsync.
        var otherOffers = await _dbContext.ListingOffers
            .Where(o => o.ListingId == offer.ListingId && o.Id != offerId && o.Status == OfferStatus.Pending)
            .ToListAsync();

        foreach (var other in otherOffers)
            other.Status = OfferStatus.Rejected;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectOfferAsync(Guid offerId, Guid sellerUserId)
    {
        var offer = await _dbContext.ListingOffers
            .Include(o => o.Listing)
            .FirstOrDefaultAsync(o => o.Id == offerId);

        if (offer is null || offer.Listing.SellerId != sellerUserId || offer.Status != OfferStatus.Pending)
            return false;

        offer.Status = OfferStatus.Rejected;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}