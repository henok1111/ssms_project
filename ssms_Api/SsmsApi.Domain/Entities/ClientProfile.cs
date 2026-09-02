using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class ClientProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string? PreferredArea { get; set; }
    public int JobsPostedCount { get; set; } = 0;
}