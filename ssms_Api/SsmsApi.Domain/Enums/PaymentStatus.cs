namespace SsmsApi.Domain.Enums;

public enum PaymentStatus
{
    Pending,     // client hasn't paid into escrow yet
    Held,        // paid, platform holding it
    Released,    // paid out to worker/supplier
    Refunded,
    Failed
}