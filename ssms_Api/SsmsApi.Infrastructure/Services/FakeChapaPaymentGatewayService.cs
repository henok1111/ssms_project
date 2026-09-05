using SsmsApi.Application.Interfaces;

namespace SsmsApi.Infrastructure.Services;

public class FakeChapaPaymentGatewayService : IPaymentGatewayService
{
    public Task<PaymentInitiationResult> InitiatePaymentAsync(decimal amount, string currency, string txRef)
    {
        var fakeCheckoutUrl = $"http://localhost:4200/fake-checkout/{txRef}";
        return Task.FromResult(new PaymentInitiationResult(true, fakeCheckoutUrl, txRef));
    }

    public Task<PaymentVerificationResult> VerifyPaymentAsync(string txRef)
    {
        return Task.FromResult(new PaymentVerificationResult(true, "success"));
    }
}