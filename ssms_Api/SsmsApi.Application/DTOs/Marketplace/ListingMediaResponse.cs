using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Application.DTOs.Marketplace;

public class ListingMediaResponse
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public ListingMediaType MediaType { get; set; }
}