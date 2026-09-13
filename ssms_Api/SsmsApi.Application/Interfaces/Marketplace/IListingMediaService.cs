using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface IListingMediaService
{
    Task<ListingMediaResponse> UploadAsync(Guid listingId, Guid sellerUserId, Stream fileStream, string fileName, string contentType);

    Task<bool> DeleteAsync(Guid mediaId, Guid sellerUserId);
}