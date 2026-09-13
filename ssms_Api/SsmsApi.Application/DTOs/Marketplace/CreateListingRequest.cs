using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Application.DTOs.Marketplace;

public class CreateListingRequest
{
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ListingCondition Condition { get; set; }
    public string Location { get; set; } = string.Empty;
}