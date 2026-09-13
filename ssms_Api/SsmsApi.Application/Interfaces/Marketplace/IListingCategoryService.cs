using SsmsApi.Application.DTOs.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface IListingCategoryService
{
    Task<IReadOnlyList<ListingCategoryResponse>> GetAllAsync();
    Task<ListingCategoryResponse> CreateAsync(CreateListingCategoryRequest request);
    Task<bool> DeleteAsync(Guid id);
}