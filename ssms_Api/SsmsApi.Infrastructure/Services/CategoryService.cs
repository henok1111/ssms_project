using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Categories;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly SsmsDbContext _dbContext;

    public CategoryService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static CategoryResponse ToResponse(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        IsServiceCategory = c.IsServiceCategory
    };

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync()
    {
        var categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
        return categories.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetByTypeAsync(bool isServiceCategory)
    {
        var categories = await _dbContext.Categories
            .Where(c => c.IsServiceCategory == isServiceCategory)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return categories.Select(ToResponse).ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        return category is null ? null : ToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var exists = await _dbContext.Categories.AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());
        if (exists)
            throw new InvalidOperationException("A category with this name already exists.");

        var category = new Category
        {
            Name = request.Name,
            IsServiceCategory = request.IsServiceCategory
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return ToResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return null;

        category.Name = request.Name;
        category.IsServiceCategory = request.IsServiceCategory;
        category.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return ToResponse(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return false;

        var inUse = await _dbContext.Jobs.AnyAsync(j => j.CategoryId == id)
            || await _dbContext.MaterialItems.AnyAsync(m => m.CategoryId == id);

        if (inUse)
            throw new InvalidOperationException("Cannot delete a category that is currently in use.");

        category.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}