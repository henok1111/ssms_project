using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public NotificationType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

    // Optional link back to whatever triggered it — nullable because
    // not every notification is tied to a specific Job (e.g. account alerts later).
    public Guid? RelatedJobId { get; set; }
}