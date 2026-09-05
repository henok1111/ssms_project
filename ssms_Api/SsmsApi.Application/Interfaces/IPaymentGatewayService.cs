namespace SsmsApi.Application.Interfaces;

public interface IPaymentGatewayService
{
    Task<PaymentInitiationResult> InitiatePaymentAsync(decimal amount, string currency, string txRef);
    Task<PaymentVerificationResult> VerifyPaymentAsync(string txRef);
}

public record PaymentInitiationResult(bool Success, string CheckoutUrl, string TxRef);
public record PaymentVerificationResult(bool Success, string Status);