using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/listing-reports")]
[Authorize]
public class ListingReportsController : ControllerBase
{
    private readonly IListingReportService _reportService;

    public ListingReportsController(IListingReportService reportService)
    {
        _reportService = reportService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("listings/{listingId:guid}")]
    public async Task<IActionResult> Report(Guid listingId, [FromBody] ReportListingRequest request)
    {
        var report = await _reportService.ReportAsync(listingId, CurrentUserId, request);
        return Ok(report);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPending()
    {
        var reports = await _reportService.GetPendingAsync();
        return Ok(reports);
    }

    [HttpPost("{reportId:guid}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Resolve(Guid reportId, [FromBody] ResolveReportRequest request)
    {
        var success = await _reportService.ResolveAsync(reportId, request);
        return success ? Ok(new { message = "Report resolved." }) : NotFound();
    }
}