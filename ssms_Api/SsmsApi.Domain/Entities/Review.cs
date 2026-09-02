using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class Review : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid ReviewerId { get; set; }        // the ApplicationUser who wrote it
    public ApplicationUser Reviewer { get; set; } = null!;

    public Guid RevieweeId { get; set; }         // the ApplicationUser being reviewed
    public ApplicationUser Reviewee { get; set; } = null!;

    public int Rating { get; set; }              // 1–5, enforced in Application layer validation
    public string? Comment { get; set; }
}