using SsmsApi.Application.DTOs.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface ISavedListingService
{
    Task<IReadOnlyList<ListingResponse>> GetMySavedAsync(Guid userId);

    Task<bool> SaveAsync(Guid userId, Guid listingId);

    Task<bool> UnsaveAsync(Guid userId, Guid listingId);
}