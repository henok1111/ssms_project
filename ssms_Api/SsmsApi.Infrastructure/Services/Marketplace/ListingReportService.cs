using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Domain.Enums.Marketplace;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services.Marketplace;

public class ListingReportService : IListingReportService
{
    private readonly SsmsDbContext _dbContext;

    public ListingReportService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static ListingReportResponse ToResponse(ListingReport r) => new()
    {
        Id = r.Id,
        ListingId = r.ListingId,
        ListingTitle = r.Listing.Title,
        ReportedById = r.ReportedById,
        ReportedByName = r.ReportedBy.FullName,
        Reason = r.Reason,
        Status = r.Status,
        CreatedAt = r.CreatedAt
    };

    public async Task<IReadOnlyList<ListingReportResponse>> GetPendingAsync()
    {
        var reports = await _dbContext.ListingReports
            .Include(r => r.Listing)
            .Include(r => r.ReportedBy)
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        return reports.Select(ToResponse).ToList();
    }

    public async Task<ListingReportResponse> ReportAsync(Guid listingId, Guid reporterUserId, ReportListingRequest request)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == listingId)
            ?? throw new InvalidOperationException("Listing not found.");

        if (listing.SellerId == reporterUserId)
            throw new InvalidOperationException("You cannot report your own listing.");

        var alreadyReported = await _dbContext.ListingReports
            .AnyAsync(r => r.ListingId == listingId && r.ReportedById == reporterUserId && r.Status == ReportStatus.Pending);
        if (alreadyReported)
            throw new InvalidOperationException("You have already reported this listing.");

        var report = new ListingReport
        {
            ListingId = listingId,
            ReportedById = reporterUserId,
            Reason = request.Reason,
            Status = ReportStatus.Pending
        };

        _dbContext.ListingReports.Add(report);
        await _dbContext.SaveChangesAsync();

        report.Listing = listing;
        report.ReportedBy = (await _dbContext.Users.FindAsync(reporterUserId))!;
        return ToResponse(report);
    }

    public async Task<bool> ResolveAsync(Guid reportId, ResolveReportRequest request)
    {
        var report = await _dbContext.ListingReports
            .Include(r => r.Listing)
            .FirstOrDefaultAsync(r => r.Id == reportId);

        if (report is null) return false;

        report.Status = request.RemoveListing ? ReportStatus.Reviewed : ReportStatus.Dismissed;

        if (request.RemoveListing)
        {
            report.Listing.Status = ListingStatus.Removed;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}