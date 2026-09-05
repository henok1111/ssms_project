using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Quotes;

public class QuoteResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public decimal LaborCost { get; set; }
    public decimal MaterialsCost { get; set; }
    public decimal TotalCost => LaborCost + MaterialsCost;
    public QuoteStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}