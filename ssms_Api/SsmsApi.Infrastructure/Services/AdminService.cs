using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Admin;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly SsmsDbContext _dbContext;
private readonly INotificationService _notificationService;

public AdminService(SsmsDbContext dbContext, INotificationService notificationService)
{
    _dbContext = dbContext;
    _notificationService = notificationService;
}
    public async Task<IReadOnlyList<PendingApprovalResponse>> GetPendingWorkersAsync()
    {
        var workers = await _dbContext.WorkerProfiles
            .Include(w => w.User)
            .Where(w => w.ApprovalStatus == ApprovalStatus.Pending)
            .ToListAsync();

        return workers.Select(w => new PendingApprovalResponse
        {
            Id = w.Id,
            UserId = w.UserId,
            FullName = w.User.FullName,
            Email = w.User.Email!,
            Role = "Worker",
            ApprovalStatus = w.ApprovalStatus,
            CreatedAt = w.CreatedAt
        }).ToList();
    }

    public async Task<IReadOnlyList<PendingApprovalResponse>> GetPendingSuppliersAsync()
    {
        var suppliers = await _dbContext.SupplierProfiles
            .Include(s => s.User)
            .Where(s => s.ApprovalStatus == ApprovalStatus.Pending)
            .ToListAsync();

        return suppliers.Select(s => new PendingApprovalResponse
        {
            Id = s.Id,
            UserId = s.UserId,
            FullName = s.User.FullName,
            Email = s.User.Email!,
            Role = "Supplier",
            ApprovalStatus = s.ApprovalStatus,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<bool> ApproveWorkerAsync(Guid workerProfileId) =>
        await SetWorkerStatus(workerProfileId, ApprovalStatus.Approved);

    public async Task<bool> RejectWorkerAsync(Guid workerProfileId) =>
        await SetWorkerStatus(workerProfileId, ApprovalStatus.Rejected);

    public async Task<bool> ApproveSupplierAsync(Guid supplierProfileId) =>
        await SetSupplierStatus(supplierProfileId, ApprovalStatus.Approved);

    public async Task<bool> RejectSupplierAsync(Guid supplierProfileId) =>
        await SetSupplierStatus(supplierProfileId, ApprovalStatus.Rejected);

   private async Task<bool> SetWorkerStatus(Guid id, ApprovalStatus status)
{
    var worker = await _dbContext.WorkerProfiles.FirstOrDefaultAsync(w => w.Id == id);
    if (worker is null) return false;

    worker.ApprovalStatus = status;
    await _dbContext.SaveChangesAsync();

    await _notificationService.CreateAsync(
        worker.UserId,
        NotificationType.OrderStatusUpdate, // same note as above — could use a dedicated type later
        status == ApprovalStatus.Approved
            ? "Your worker account has been approved! You can now apply to jobs."
            : "Your worker account application was rejected."
    );

    return true;
}
    private async Task<bool> SetSupplierStatus(Guid id, ApprovalStatus status)
{
    var supplier = await _dbContext.SupplierProfiles.FirstOrDefaultAsync(s => s.Id == id);
    if (supplier is null) return false;

    supplier.ApprovalStatus = status;
    await _dbContext.SaveChangesAsync();

    await _notificationService.CreateAsync(
        supplier.UserId,
        NotificationType.OrderStatusUpdate,
        status == ApprovalStatus.Approved
            ? "Your supplier account has been approved! You can now list materials."
            : "Your supplier account application was rejected."
    );

    return true;
}
    public async Task<IReadOnlyList<UserSummaryResponse>> GetUsersAsync(UserRole? role)
{
    var query = _dbContext.Users
        .Include(u => u.WorkerProfile)
        .Include(u => u.SupplierProfile)
        .AsQueryable();

    if (role.HasValue)
        query = query.Where(u => u.Role == role.Value);

    var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();

    return users.Select(u => new UserSummaryResponse
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email!,
        Role = u.Role.ToString(),
        IsActive = u.IsActive,
        ApprovalStatus = u.WorkerProfile?.ApprovalStatus.ToString() ?? u.SupplierProfile?.ApprovalStatus.ToString(),
        CreatedAt = u.CreatedAt
    }).ToList();
}

public async Task<bool> DeactivateUserAsync(Guid userId)
{
    var user = await _dbContext.Users.FindAsync(userId);
    if (user is null) return false;

    user.IsActive = false;
    await _dbContext.SaveChangesAsync();
    return true;
}

public async Task<bool> ReactivateUserAsync(Guid userId)
{
    var user = await _dbContext.Users.FindAsync(userId);
    if (user is null) return false;

    user.IsActive = true;
    await _dbContext.SaveChangesAsync();
    return true;
}
}