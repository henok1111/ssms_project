using Microsoft.AspNetCore.Identity;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WorkerProfile? WorkerProfile { get; set; }
    public ClientProfile? ClientProfile { get; set; }
    public SupplierProfile? SupplierProfile { get; set; }
}