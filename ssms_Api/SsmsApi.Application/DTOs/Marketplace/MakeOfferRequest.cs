namespace SsmsApi.Application.DTOs.Marketplace;

public class MakeOfferRequest
{
    public decimal OfferedPrice { get; set; }
    public string? Message { get; set; }
}