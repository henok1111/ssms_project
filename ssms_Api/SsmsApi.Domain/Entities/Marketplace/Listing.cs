using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Domain.Entities.Marketplace;

public class Listing : BaseEntity
{
    public Guid SellerId { get; set; }
    public ApplicationUser Seller { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public ListingCategory Category { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ListingCondition Condition { get; set; }
    public string Location { get; set; } = string.Empty;
    public ListingStatus Status { get; set; } = ListingStatus.Active;
public int ViewCount { get; set; } = 0;
public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(60);
public ICollection<ListingMedia> Media { get; set; } = new List<ListingMedia>();
    public ICollection<ListingOffer> Offers { get; set; } = new List<ListingOffer>();
}