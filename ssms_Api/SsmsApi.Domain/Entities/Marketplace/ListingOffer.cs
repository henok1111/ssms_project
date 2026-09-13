using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Domain.Entities.Marketplace;

public class ListingOffer : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid BuyerId { get; set; }
    public ApplicationUser Buyer { get; set; } = null!;

    public decimal OfferedPrice { get; set; }
    public string? Message { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
}