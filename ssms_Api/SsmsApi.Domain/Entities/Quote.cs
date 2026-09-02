using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class Quote : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public decimal LaborCost { get; set; }
    public decimal MaterialsCost { get; set; }
    public decimal TotalCost => LaborCost + MaterialsCost;

    public QuoteStatus Status { get; set; } = QuoteStatus.PendingApproval;
}
