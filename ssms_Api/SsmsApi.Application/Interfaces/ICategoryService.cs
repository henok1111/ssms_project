using SsmsApi.Application.DTOs.Categories;

namespace SsmsApi.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync();

    Task<IReadOnlyList<CategoryResponse>> GetByTypeAsync(bool isServiceCategory);

    Task<CategoryResponse?> GetByIdAsync(Guid id);

    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);

    Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request);

    Task<bool> DeleteAsync(Guid id);
}