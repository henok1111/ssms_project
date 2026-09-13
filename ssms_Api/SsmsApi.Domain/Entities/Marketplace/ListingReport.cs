using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums.Marketplace;

namespace SsmsApi.Domain.Entities.Marketplace;

public class ListingReport : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid ReportedById { get; set; }
    public ApplicationUser ReportedBy { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
}