using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Disputes;

public class DisputeResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid RaisedById { get; set; }
    public string RaisedByName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DisputeStatus Status { get; set; }
    public string? AdminResolutionNote { get; set; }
    public DateTime CreatedAt { get; set; }
}