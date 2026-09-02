using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class JobAttachment : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public bool IsAiAnalyzed { get; set; } = false;
}