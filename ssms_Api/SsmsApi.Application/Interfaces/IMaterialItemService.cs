using SsmsApi.Application.DTOs.Materials;

namespace SsmsApi.Application.Interfaces;

public interface IMaterialItemService
{
    Task<MaterialItemResponse?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<MaterialItemResponse>> GetAllAsync();

    Task<IReadOnlyList<MaterialItemResponse>> GetBySupplierAsync(Guid supplierUserId);

    Task<IReadOnlyList<MaterialItemResponse>> SearchAsync(Guid? categoryId, string? name);

    Task<MaterialItemResponse> CreateAsync(Guid supplierUserId, CreateMaterialItemRequest request);

    Task<MaterialItemResponse?> UpdateAsync(Guid id, Guid supplierUserId, UpdateMaterialItemRequest request);

    Task<bool> DeleteAsync(Guid id, Guid supplierUserId);
}