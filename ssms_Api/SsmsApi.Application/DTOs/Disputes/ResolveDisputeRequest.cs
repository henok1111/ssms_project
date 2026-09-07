namespace SsmsApi.Application.DTOs.Disputes;

public class ResolveDisputeRequest
{
    public string AdminResolutionNote { get; set; } = string.Empty;
    public bool Approve { get; set; } // true = Resolved (dispute upheld), false = Rejected (dismissed)
}