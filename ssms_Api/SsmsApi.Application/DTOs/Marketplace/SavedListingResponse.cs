using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Application.DTOs.Marketplace;

public class SavedListingResponse
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ListingCondition Condition { get; set; }
    public string Location { get; set; } = string.Empty;
    public ListingStatus Status { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    
    public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
}