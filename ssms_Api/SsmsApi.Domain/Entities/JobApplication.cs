using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class JobApplication : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid WorkerId { get; set; }
    public WorkerProfile Worker { get; set; } = null!;

    public decimal ProposedPrice { get; set; }
    public string? Message { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
}