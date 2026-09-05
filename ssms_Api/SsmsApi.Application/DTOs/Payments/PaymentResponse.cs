using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Payments;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public decimal Amount { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal AmountReleasedToWorker { get; set; }
    public PaymentStatus Status { get; set; }
    public string TxRef { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}