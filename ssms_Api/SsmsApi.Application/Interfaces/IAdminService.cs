using SsmsApi.Application.DTOs.Admin;

namespace SsmsApi.Application.Interfaces;

public interface IAdminService
{
    Task<IReadOnlyList<PendingApprovalResponse>> GetPendingWorkersAsync();
    Task<IReadOnlyList<PendingApprovalResponse>> GetPendingSuppliersAsync();
    Task<bool> ApproveWorkerAsync(Guid workerProfileId);
    Task<bool> RejectWorkerAsync(Guid workerProfileId);
    Task<bool> ApproveSupplierAsync(Guid supplierProfileId);
    Task<bool> RejectSupplierAsync(Guid supplierProfileId);
}