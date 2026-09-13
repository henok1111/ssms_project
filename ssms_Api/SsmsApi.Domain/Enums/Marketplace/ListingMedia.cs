using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Domain.Entities.Marketplace;

public class ListingMedia : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public string FileUrl { get; set; } = string.Empty;
    public ListingMediaType MediaType { get; set; }
}