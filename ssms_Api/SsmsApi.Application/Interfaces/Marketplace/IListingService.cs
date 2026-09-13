using SsmsApi.Application.DTOs.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface IListingService
{
    Task<ListingResponse?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<ListingResponse>> GetActiveAsync();

    Task<IReadOnlyList<ListingResponse>> SearchAsync(Guid? categoryId, string? keyword, decimal? maxPrice, string? location, string? sortBy);
    Task<IReadOnlyList<ListingResponse>> GetMyListingsAsync(Guid sellerUserId);

    Task<ListingResponse> CreateAsync(Guid sellerUserId, CreateListingRequest request);

    Task<ListingResponse?> UpdateAsync(Guid id, Guid sellerUserId, UpdateListingRequest request);

    Task<bool> MarkAsSoldAsync(Guid id, Guid sellerUserId);

    Task<bool> DeleteAsync(Guid id, Guid sellerUserId);
}