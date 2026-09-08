using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Materials;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Enums;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class MaterialOrderService : IMaterialOrderService
{
    private readonly SsmsDbContext _dbContext;

    public MaterialOrderService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static IQueryable<Domain.Entities.MaterialOrder> BaseQuery(SsmsDbContext db) =>
        db.MaterialOrders
            .Include(o => o.Supplier)
            .Include(o => o.JobMaterialRequest).ThenInclude(r => r.MaterialItem);

    private static MaterialOrderResponse ToResponse(Domain.Entities.MaterialOrder o) => new()
    {
        Id = o.Id,
        JobMaterialRequestId = o.JobMaterialRequestId,
        MaterialItemName = o.JobMaterialRequest.MaterialItem.Name,
        SupplierId = o.SupplierId,
        SupplierShopName = o.Supplier.ShopName,
        QuantityOrdered = o.QuantityOrdered,
        TotalPrice = o.TotalPrice,
        Status = o.Status,
        CreatedAt = o.CreatedAt
    };

    public async Task<IReadOnlyList<MaterialOrderResponse>> GetForSupplierAsync(Guid supplierUserId)
    {
        var orders = await BaseQuery(_dbContext)
            .Where(o => o.Supplier.UserId == supplierUserId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<MaterialOrderResponse>> GetForJobAsync(Guid jobId)
    {
        var orders = await BaseQuery(_dbContext)
            .Where(o => o.JobMaterialRequest.JobId == jobId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(ToResponse).ToList();
    }

    public async Task<bool> ConfirmAsync(Guid orderId, Guid supplierUserId)
    {
        var order = await _dbContext.MaterialOrders
            .Include(o => o.Supplier)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null || order.Supplier.UserId != supplierUserId) return false;
        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only Pending orders can be confirmed.");

        order.Status = OrderStatus.Confirmed;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> FulfillAsync(Guid orderId, Guid supplierUserId)
    {
        var order = await _dbContext.MaterialOrders
            .Include(o => o.Supplier)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null || order.Supplier.UserId != supplierUserId) return false;
        if (order.Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only Confirmed orders can be marked Fulfilled.");

        order.Status = OrderStatus.Fulfilled;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid orderId, Guid supplierUserId)
    {
        var order = await _dbContext.MaterialOrders
            .Include(o => o.Supplier)
            .Include(o => o.JobMaterialRequest).ThenInclude(r => r.MaterialItem)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null || order.Supplier.UserId != supplierUserId) return false;
        if (order.Status == OrderStatus.Fulfilled)
            throw new InvalidOperationException("A Fulfilled order cannot be cancelled.");

        order.Status = OrderStatus.Cancelled;

        // Restock — the stock was deducted when the order was created (Quote approval),
        // so cancelling should give it back.
        order.JobMaterialRequest.MaterialItem.StockQuantity += order.QuantityOrdered;

        await _dbContext.SaveChangesAsync();
        return true;
    }
}