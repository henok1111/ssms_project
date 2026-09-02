using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class MaterialOrder : BaseEntity
{
    public Guid JobMaterialRequestId { get; set; }
    public JobMaterialRequest JobMaterialRequest { get; set; } = null!;

    public Guid SupplierId { get; set; }
    public SupplierProfile Supplier { get; set; } = null!;

    public int QuantityOrdered { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}