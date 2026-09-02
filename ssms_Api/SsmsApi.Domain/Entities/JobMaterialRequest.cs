using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

// Links a Job to a specific MaterialItem it needs, with quantity.
// One Job can have many of these — one row per material needed.
public class JobMaterialRequest : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid MaterialItemId { get; set; }
    public MaterialItem MaterialItem { get; set; } = null!;

    public int QuantityNeeded { get; set; }

    // Denormalized snapshot of price at request time — see explanation below.
    public decimal UnitPriceAtRequest { get; set; }
}