namespace SsmsApi.Application.DTOs.Attachments;

public class JobAttachmentResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public bool IsAiAnalyzed { get; set; }
    public DateTime CreatedAt { get; set; }
}