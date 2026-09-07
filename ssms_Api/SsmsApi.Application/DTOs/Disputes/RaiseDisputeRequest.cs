namespace SsmsApi.Application.DTOs.Disputes;

public class RaiseDisputeRequest
{
    public Guid JobId { get; set; }
    public string Reason { get; set; } = string.Empty;
}