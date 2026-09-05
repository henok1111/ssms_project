namespace SsmsApi.Application.DTOs.Reviews;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public Guid RevieweeId { get; set; }
    public string RevieweeName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}