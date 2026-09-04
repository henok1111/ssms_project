using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public WorkerType? WorkerType { get; set; }
    public string? ServiceArea { get; set; }

    public string? ShopName { get; set; }
    public string? SupplierLocation { get; set; }
}