using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Materials;

public class MaterialOrderResponse
{
    public Guid Id { get; set; }
    public Guid JobMaterialRequestId { get; set; }
    public string MaterialItemName { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierShopName { get; set; } = string.Empty;
    public int QuantityOrdered { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}