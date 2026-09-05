namespace SsmsApi.Application.DTOs.Reviews;

public class CreateReviewRequest
{
    public Guid JobId { get; set; }
    public Guid RevieweeId { get; set; } // the ApplicationUser being reviewed
    public int Rating { get; set; }
    public string? Comment { get; set; }
}