using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class WorkerProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public WorkerType WorkerType { get; set; }
    public string? Bio { get; set; }
    public string? ServiceArea { get; set; }
    public bool IsAvailable { get; set; } = false;
    public decimal RatingAverage { get; set; } = 0;
    public int CompletedJobsCount { get; set; } = 0;

    public ICollection<WorkerSkill> Skills { get; set; } = new List<WorkerSkill>();
}