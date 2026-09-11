using SsmsApi.Application.DTOs.Materials;

namespace SsmsApi.Application.Interfaces;

public interface IMaterialOrderService
{
    Task<IReadOnlyList<MaterialOrderResponse>> GetForSupplierAsync(Guid supplierUserId);

    Task<IReadOnlyList<MaterialOrderResponse>> GetForJobAsync(Guid jobId);

    Task<bool> ConfirmAsync(Guid orderId, Guid supplierUserId);

    Task<bool> FulfillAsync(Guid orderId, Guid supplierUserId);

    Task<bool> CancelAsync(Guid orderId, Guid supplierUserId);
}