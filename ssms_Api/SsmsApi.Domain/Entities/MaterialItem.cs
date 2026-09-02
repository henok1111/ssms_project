using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class MaterialItem : BaseEntity
{
    public Guid SupplierId { get; set; }
    public SupplierProfile Supplier { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;   // "kg", "piece", "liter", "meter"
    public decimal PricePerUnit { get; set; }
    public int StockQuantity { get; set; }
}