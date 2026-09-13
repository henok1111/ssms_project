namespace SsmsApi.Application.DTOs.Marketplace;

public class ResolveReportRequest
{
    public bool RemoveListing { get; set; } // true = the report was valid, take the listing down
}