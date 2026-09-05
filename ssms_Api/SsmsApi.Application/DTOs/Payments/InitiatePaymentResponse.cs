namespace SsmsApi.Application.DTOs.Payments;

public class InitiatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public string CheckoutUrl { get; set; } = string.Empty;
    public string TxRef { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}