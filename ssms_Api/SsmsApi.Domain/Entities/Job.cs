using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class Job : BaseEntity
{
    public Guid ClientId { get; set; }
    public ClientProfile Client { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public JobType JobType { get; set; }
    public string? Location { get; set; }       // null when JobType == Remote

    public decimal Budget { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Open;

    // Set once a worker's application is accepted — nullable because
    // an Open job has no assigned worker yet.
    public Guid? AssignedWorkerId { get; set; }
    public WorkerProfile? AssignedWorker { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}