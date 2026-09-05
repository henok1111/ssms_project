using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Admin;

public class PendingApprovalResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Worker" or "Supplier"
    public ApprovalStatus ApprovalStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}