using SsmsApi.Application.DTOs.Admin;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Application.Interfaces;

public interface IAdminService
{
    Task<IReadOnlyList<PendingApprovalResponse>> GetPendingWorkersAsync();
    Task<IReadOnlyList<PendingApprovalResponse>> GetPendingSuppliersAsync();
    Task<bool> ApproveWorkerAsync(Guid workerProfileId);
    Task<bool> RejectWorkerAsync(Guid workerProfileId);
    Task<bool> ApproveSupplierAsync(Guid supplierProfileId);
    Task<bool> RejectSupplierAsync(Guid supplierProfileId);
    Task<IReadOnlyList<UserSummaryResponse>> GetUsersAsync(UserRole? role);
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<bool> ReactivateUserAsync(Guid userId);
}