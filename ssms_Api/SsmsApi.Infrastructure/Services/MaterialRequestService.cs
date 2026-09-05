using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Materials;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services;

public class MaterialRequestService : IMaterialRequestService
{
    private readonly SsmsDbContext _dbContext;

    public MaterialRequestService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static MaterialRequestResponse ToResponse(JobMaterialRequest r) => new()
    {
        Id = r.Id,
        JobId = r.JobId,
        MaterialItemId = r.MaterialItemId,
        MaterialItemName = r.MaterialItem.Name,
        SupplierShopName = r.MaterialItem.Supplier.ShopName,
        QuantityNeeded = r.QuantityNeeded,
        UnitPriceAtRequest = r.UnitPriceAtRequest
    };

    public async Task<IReadOnlyList<MaterialRequestResponse>> GetForJobAsync(Guid jobId)
    {
        var requests = await _dbContext.JobMaterialRequests
            .Include(r => r.MaterialItem).ThenInclude(m => m.Supplier)
            .Where(r => r.JobId == jobId)
            .ToListAsync();

        return requests.Select(ToResponse).ToList();
    }

    public async Task<MaterialRequestResponse> AddAsync(Guid jobId, Guid workerUserId, AddMaterialRequestRequest request)
    {
        var job = await _dbContext.Jobs
            .Include(j => j.AssignedWorker)
            .FirstOrDefaultAsync(j => j.Id == jobId)
            ?? throw new InvalidOperationException("Job not found.");

        if (job.AssignedWorker is null || job.AssignedWorker.UserId != workerUserId)
            throw new UnauthorizedAccessException("Only the assigned worker can add material requests.");

        var materialItem = await _dbContext.MaterialItems
            .Include(m => m.Supplier)
            .FirstOrDefaultAsync(m => m.Id == request.MaterialItemId)
            ?? throw new InvalidOperationException("Material item not found.");

        if (materialItem.StockQuantity < request.QuantityNeeded)
            throw new InvalidOperationException("Requested quantity exceeds available stock.");

        var materialRequest = new JobMaterialRequest
        {
            JobId = jobId,
            MaterialItemId = materialItem.Id,
            QuantityNeeded = request.QuantityNeeded,
            // Snapshotting price NOW, at request time — this is the exact
            // reasoning we discussed: protects against the supplier changing
            // PricePerUnit later before the order is actually placed.
            UnitPriceAtRequest = materialItem.PricePerUnit
        };

        _dbContext.JobMaterialRequests.Add(materialRequest);
        await _dbContext.SaveChangesAsync();

        materialRequest.MaterialItem = materialItem;
        return ToResponse(materialRequest);
    }

    public async Task<bool> RemoveAsync(Guid requestId, Guid workerUserId)
    {
        var request = await _dbContext.JobMaterialRequests
            .Include(r => r.Job).ThenInclude(j => j.AssignedWorker)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null || request.Job.AssignedWorker?.UserId != workerUserId)
            return false;

        request.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}