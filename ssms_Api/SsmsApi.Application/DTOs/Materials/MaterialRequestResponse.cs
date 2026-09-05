namespace SsmsApi.Application.DTOs.Materials;

public class MaterialRequestResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid MaterialItemId { get; set; }
    public string MaterialItemName { get; set; } = string.Empty;
    public string SupplierShopName { get; set; } = string.Empty;
    public int QuantityNeeded { get; set; }
    public decimal UnitPriceAtRequest { get; set; }
    public decimal LineTotal => QuantityNeeded * UnitPriceAtRequest;
}