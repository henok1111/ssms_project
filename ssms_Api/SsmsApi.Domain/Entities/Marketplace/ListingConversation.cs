using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities.Marketplace;

public class ListingConversation : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid BuyerId { get; set; }
    public ApplicationUser Buyer { get; set; } = null!;

    public ICollection<ListingMessage> Messages { get; set; } = new List<ListingMessage>();
}