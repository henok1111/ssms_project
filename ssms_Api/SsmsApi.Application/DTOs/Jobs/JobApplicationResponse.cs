using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Jobs;

public class JobApplicationResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;   // new
    public decimal ProposedPrice { get; set; }
    public string? Message { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}