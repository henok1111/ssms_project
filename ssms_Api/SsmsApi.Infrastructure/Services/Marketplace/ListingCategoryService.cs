using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services.Marketplace;

public class ListingCategoryService : IListingCategoryService
{
    private readonly SsmsDbContext _dbContext;

    public ListingCategoryService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ListingCategoryResponse>> GetAllAsync()
    {
        var categories = await _dbContext.ListingCategories.OrderBy(c => c.Name).ToListAsync();
        return categories.Select(c => new ListingCategoryResponse { Id = c.Id, Name = c.Name }).ToList();
    }

    public async Task<ListingCategoryResponse> CreateAsync(CreateListingCategoryRequest request)
    {
        var exists = await _dbContext.ListingCategories.AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());
        if (exists)
            throw new InvalidOperationException("This category already exists.");

        var category = new ListingCategory { Name = request.Name };
        _dbContext.ListingCategories.Add(category);
        await _dbContext.SaveChangesAsync();

        return new ListingCategoryResponse { Id = category.Id, Name = category.Name };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _dbContext.ListingCategories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return false;

        var inUse = await _dbContext.Listings.AnyAsync(l => l.CategoryId == id);
        if (inUse)
            throw new InvalidOperationException("Cannot delete a category currently in use by listings.");

        category.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}