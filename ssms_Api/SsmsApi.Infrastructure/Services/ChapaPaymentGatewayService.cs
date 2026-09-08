using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Infrastructure.Services;

public class ChapaPaymentGatewayService : IPaymentGatewayService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ChapaPaymentGatewayService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _httpClient.BaseAddress = new Uri("https://api.chapa.co/v1/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["Chapa:SecretKey"]);
    }

    public async Task<PaymentInitiationResult> InitiatePaymentAsync(decimal amount, string currency, string txRef)
    {
        var payload = new
        {
            amount = amount.ToString("F2"),
            currency,
            tx_ref = txRef,
            callback_url = _configuration["Chapa:CallbackUrl"],
            return_url = _configuration["Chapa:ReturnUrl"]
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("transaction/initialize", content);
        var responseBody = await response.Content.ReadAsStringAsync();
Console.WriteLine();
Console.WriteLine("========== CHAPA RESPONSE ==========");
Console.WriteLine($"Status Code: {(int)response.StatusCode}");
Console.WriteLine($"Is Success: {response.IsSuccessStatusCode}");
Console.WriteLine($"Response Body: {responseBody}");
Console.WriteLine("====================================");
Console.WriteLine();
        if (!response.IsSuccessStatusCode)
            return new PaymentInitiationResult(false, string.Empty, txRef);

        using var doc = JsonDocument.Parse(responseBody);
        var checkoutUrl = doc.RootElement
            .GetProperty("data")
            .GetProperty("checkout_url")
            .GetString() ?? string.Empty;

        return new PaymentInitiationResult(true, checkoutUrl, txRef);
    }

    public async Task<PaymentVerificationResult> VerifyPaymentAsync(string txRef)
    {
        var response = await _httpClient.GetAsync($"transaction/verify/{txRef}");
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return new PaymentVerificationResult(false, "failed");

        using var doc = JsonDocument.Parse(responseBody);
        var status = doc.RootElement
            .GetProperty("data")
            .GetProperty("status")
            .GetString() ?? "failed";

        return new PaymentVerificationResult(status == "success", status);
    }
}