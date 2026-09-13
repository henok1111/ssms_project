using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Application.DTOs.Marketplace;

public class ListingReportResponse
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public Guid ReportedById { get; set; }
    public string ReportedByName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}