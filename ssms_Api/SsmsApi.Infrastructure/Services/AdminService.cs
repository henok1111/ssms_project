using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Admin;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly SsmsDbContext _dbContext;

    public AdminService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
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
        return true;
    }

    private async Task<bool> SetSupplierStatus(Guid id, ApprovalStatus status)
    {
        var supplier = await _dbContext.SupplierProfiles.FirstOrDefaultAsync(s => s.Id == id);
        if (supplier is null) return false;

        supplier.ApprovalStatus = status;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}