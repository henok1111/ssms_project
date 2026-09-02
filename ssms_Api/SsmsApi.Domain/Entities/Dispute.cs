using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class Dispute : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid RaisedById { get; set; }
    public ApplicationUser RaisedBy { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;
    public DisputeStatus Status { get; set; } = DisputeStatus.Open;
    public string? AdminResolutionNote { get; set; }
}