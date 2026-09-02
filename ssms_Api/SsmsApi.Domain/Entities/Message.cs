using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class Message : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}