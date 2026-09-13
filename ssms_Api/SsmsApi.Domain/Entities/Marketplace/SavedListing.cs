using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities.Marketplace;

public class SavedListing : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
}