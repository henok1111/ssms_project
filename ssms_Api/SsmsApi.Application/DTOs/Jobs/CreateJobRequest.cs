using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Jobs;

public class CreateJobRequest
{
   

    public Guid CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public JobType JobType { get; set; }

    public string? Location { get; set; }

    public decimal Budget { get; set; }

    public string CategoryName { get; set; } = string.Empty;
public string ClientName { get; set; } = string.Empty;
public string? AssignedWorkerName { get; set; }
}
