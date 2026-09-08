namespace SsmsApi.Application.DTOs.Admin;

public class UserSummaryResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}