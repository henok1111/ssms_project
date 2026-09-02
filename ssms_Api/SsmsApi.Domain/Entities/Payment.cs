using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public Guid QuoteId { get; set; }
    public Quote Quote { get; set; } = null!;

    public decimal Amount { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal AmountReleasedToWorker => Amount - PlatformCommission;

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Chapa-shaped fields — kept generic (GatewayProvider) so this entity
    // isn't locked to Chapa specifically if you ever add another provider.
    public string GatewayProvider { get; set; } = "Chapa";
    public string TxRef { get; set; } = string.Empty;       // matches Chapa's tx_ref
    public string? CheckoutUrl { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}