namespace SsmsApi.Application.DTOs.Materials;

public class CreateMaterialItemRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public int StockQuantity { get; set; }
}