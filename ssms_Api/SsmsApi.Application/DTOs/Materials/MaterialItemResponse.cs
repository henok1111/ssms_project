namespace SsmsApi.Application.DTOs.Materials;

public class MaterialItemResponse
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierShopName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}