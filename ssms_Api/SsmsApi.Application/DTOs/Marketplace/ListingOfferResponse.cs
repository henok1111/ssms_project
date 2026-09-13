using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Application.DTOs.Marketplace;

public class ListingOfferResponse
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid BuyerId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal OfferedPrice { get; set; }
    public string? Message { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}