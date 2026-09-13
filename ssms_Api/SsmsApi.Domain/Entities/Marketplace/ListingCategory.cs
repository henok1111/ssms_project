using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities.Marketplace;

public class ListingCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}