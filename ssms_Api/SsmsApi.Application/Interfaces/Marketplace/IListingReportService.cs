using SsmsApi.Application.DTOs.Marketplace;

namespace SsmsApi.Application.Interfaces.Marketplace;

public interface IListingReportService
{
    Task<IReadOnlyList<ListingReportResponse>> GetPendingAsync();

    Task<ListingReportResponse> ReportAsync(Guid listingId, Guid reporterUserId, ReportListingRequest request);

    Task<bool> ResolveAsync(Guid reportId, ResolveReportRequest request);
}