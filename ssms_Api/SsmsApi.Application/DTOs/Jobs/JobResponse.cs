using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Jobs;

public class JobResponse
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public Guid CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public JobType JobType { get; set; }

    public string? Location { get; set; }

    public decimal Budget { get; set; }

    public JobStatus Status { get; set; }

    public Guid? AssignedWorkerId { get; set; }

    public DateTime CreatedAt { get; set; }
}
