using SsmsApi.Application.DTOs.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface IListingOfferService
{
    Task<IReadOnlyList<ListingOfferResponse>> GetForListingAsync(Guid listingId, Guid sellerUserId);

    Task<IReadOnlyList<ListingOfferResponse>> GetMyOffersAsync(Guid buyerUserId);

    Task<ListingOfferResponse> MakeOfferAsync(Guid listingId, Guid buyerUserId, MakeOfferRequest request);

    Task<bool> AcceptOfferAsync(Guid offerId, Guid sellerUserId);

    Task<bool> RejectOfferAsync(Guid offerId, Guid sellerUserId);
}