namespace SsmsApi.Application.DTOs.Materials;

public class AddMaterialRequestRequest
{
    public Guid MaterialItemId { get; set; }
    public int QuantityNeeded { get; set; }
}