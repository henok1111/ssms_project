using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Materials;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class MaterialItemService : IMaterialItemService
{
    private readonly SsmsDbContext _dbContext;

    public MaterialItemService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static IQueryable<MaterialItem> BaseQuery(SsmsDbContext db) =>
        db.MaterialItems
            .Include(m => m.Supplier)
            .Include(m => m.Category);

    private static MaterialItemResponse ToResponse(MaterialItem item) => new()
    {
        Id = item.Id,
        SupplierId = item.SupplierId,
        SupplierShopName = item.Supplier.ShopName,
        CategoryId = item.CategoryId,
        CategoryName = item.Category.Name,
        Name = item.Name,
        Unit = item.Unit,
        PricePerUnit = item.PricePerUnit,
        StockQuantity = item.StockQuantity,
        CreatedAt = item.CreatedAt
    };

    public async Task<MaterialItemResponse?> GetByIdAsync(Guid id)
    {
        var item = await BaseQuery(_dbContext).FirstOrDefaultAsync(m => m.Id == id);
        return item is null ? null : ToResponse(item);
    }

    public async Task<IReadOnlyList<MaterialItemResponse>> GetAllAsync()
    {
        var items = await BaseQuery(_dbContext).OrderByDescending(m => m.CreatedAt).ToListAsync();
        return items.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<MaterialItemResponse>> GetBySupplierAsync(Guid supplierUserId)
    {
        var items = await BaseQuery(_dbContext)
            .Where(m => m.Supplier.UserId == supplierUserId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return items.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<MaterialItemResponse>> SearchAsync(Guid? categoryId, string? name)
    {
        var query = BaseQuery(_dbContext).Where(m => m.StockQuantity > 0);

        if (categoryId.HasValue)
            query = query.Where(m => m.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(m => m.Name.Contains(name));

        var items = await query.OrderBy(m => m.Name).ToListAsync();
        return items.Select(ToResponse).ToList();
    }

    public async Task<MaterialItemResponse> CreateAsync(Guid supplierUserId, CreateMaterialItemRequest request)
    {
        var supplierProfile = await _dbContext.SupplierProfiles
            .FirstOrDefaultAsync(s => s.UserId == supplierUserId)
            ?? throw new InvalidOperationException("Supplier profile not found.");

        var item = new MaterialItem
        {
            SupplierId = supplierProfile.Id,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Unit = request.Unit,
            PricePerUnit = request.PricePerUnit,
            StockQuantity = request.StockQuantity
        };

        _dbContext.MaterialItems.Add(item);
        await _dbContext.SaveChangesAsync();

        var created = await BaseQuery(_dbContext).FirstAsync(m => m.Id == item.Id);
        return ToResponse(created);
    }

    public async Task<MaterialItemResponse?> UpdateAsync(Guid id, Guid supplierUserId, UpdateMaterialItemRequest request)
    {
        var item = await _dbContext.MaterialItems
            .Include(m => m.Supplier)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (item is null || item.Supplier.UserId != supplierUserId)
            return null;

        item.CategoryId = request.CategoryId;
        item.Name = request.Name;
        item.Unit = request.Unit;
        item.PricePerUnit = request.PricePerUnit;
        item.StockQuantity = request.StockQuantity;
        item.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var updated = await BaseQuery(_dbContext).FirstAsync(m => m.Id == item.Id);
        return ToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid supplierUserId)
    {
        var item = await _dbContext.MaterialItems
            .Include(m => m.Supplier)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (item is null || item.Supplier.UserId != supplierUserId)
            return false;

        item.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}