using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Domain.Enums.Marketplace;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services.Marketplace;

public class ListingService : IListingService
{
    private readonly SsmsDbContext _dbContext;

    public ListingService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static IQueryable<Listing> BaseQuery(SsmsDbContext db) =>
        db.Listings
            .Include(l => l.Seller)
            .Include(l => l.Category)
            .Include(l => l.Media);

    private static ListingResponse ToResponse(Listing l) => new()
    {
        Id = l.Id,
        SellerId = l.SellerId,
        SellerName = l.Seller.FullName,
        CategoryId = l.CategoryId,
        CategoryName = l.Category.Name,
        Title = l.Title,
        Description = l.Description,
        Price = l.Price,
        Condition = l.Condition,
        Location = l.Location,
        Status = l.Status,
         ImageUrls = l.Media.Where(m => m.MediaType == ListingMediaType.Image).Select(m => m.FileUrl).ToList(),
    AudioUrls = l.Media.Where(m => m.MediaType == ListingMediaType.Audio).Select(m => m.FileUrl).ToList(),

        CreatedAt = l.CreatedAt,
        ViewCount = l.ViewCount
    };

   public async Task<ListingResponse?> GetByIdAsync(Guid id)
{
    var listing = await BaseQuery(_dbContext).FirstOrDefaultAsync(l => l.Id == id);
    if (listing is null) return null;

    listing.ViewCount++;
    await _dbContext.SaveChangesAsync();

    return ToResponse(listing);
}
    public async Task<IReadOnlyList<ListingResponse>> GetActiveAsync()
    {
         await ExpireStaleListingsAsync(); 
        var listings = await BaseQuery(_dbContext)
            .Where(l => l.Status == ListingStatus.Active)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return listings.Select(ToResponse).ToList();
    }

   public async Task<IReadOnlyList<ListingResponse>> SearchAsync(
    Guid? categoryId, string? keyword, decimal? maxPrice, string? location, string? sortBy)
{
    await ExpireStaleListingsAsync();
    var query = BaseQuery(_dbContext).Where(l => l.Status == ListingStatus.Active);

    if (categoryId.HasValue)
        query = query.Where(l => l.CategoryId == categoryId.Value);

    if (!string.IsNullOrWhiteSpace(keyword))
        query = query.Where(l => l.Title.Contains(keyword) || l.Description.Contains(keyword));

    if (maxPrice.HasValue)
        query = query.Where(l => l.Price <= maxPrice.Value);

    if (!string.IsNullOrWhiteSpace(location))
        query = query.Where(l => l.Location.Contains(location));

    query = sortBy switch
    {
        "price_asc" => query.OrderBy(l => l.Price),
        "price_desc" => query.OrderByDescending(l => l.Price),
        _ => query.OrderByDescending(l => l.CreatedAt)
    };

    var listings = await query.ToListAsync();
    return listings.Select(ToResponse).ToList();
}
    public async Task<IReadOnlyList<ListingResponse>> GetMyListingsAsync(Guid sellerUserId)
    {
        var listings = await BaseQuery(_dbContext)
            .Where(l => l.SellerId == sellerUserId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return listings.Select(ToResponse).ToList();
    }

    public async Task<ListingResponse> CreateAsync(Guid sellerUserId, CreateListingRequest request)
    {
        var listing = new Listing
        {
            SellerId = sellerUserId,
            CategoryId = request.CategoryId,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Condition = request.Condition,
            Location = request.Location,
            Status = ListingStatus.Active
        };

        _dbContext.Listings.Add(listing);
        await _dbContext.SaveChangesAsync();

        var created = await BaseQuery(_dbContext).FirstAsync(l => l.Id == listing.Id);
        return ToResponse(created);
    }

    public async Task<ListingResponse?> UpdateAsync(Guid id, Guid sellerUserId, UpdateListingRequest request)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id);

        if (listing is null || listing.SellerId != sellerUserId)
            return null;

        listing.CategoryId = request.CategoryId;
        listing.Title = request.Title;
        listing.Description = request.Description;
        listing.Price = request.Price;
        listing.Condition = request.Condition;
        listing.Location = request.Location;
        listing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var updated = await BaseQuery(_dbContext).FirstAsync(l => l.Id == listing.Id);
        return ToResponse(updated);
    }

    public async Task<bool> MarkAsSoldAsync(Guid id, Guid sellerUserId)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id);

        if (listing is null || listing.SellerId != sellerUserId) return false;

        listing.Status = ListingStatus.Sold;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid sellerUserId)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id);

        if (listing is null || listing.SellerId != sellerUserId) return false;

        listing.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }


    public async Task<bool> MarkAsReservedAsync(Guid id, Guid sellerUserId)
{
    var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id);
    if (listing is null || listing.SellerId != sellerUserId) return false;
    if (listing.Status != ListingStatus.Active) return false;

    listing.Status = ListingStatus.Reserved;
    await _dbContext.SaveChangesAsync();
    return true;
}

private async Task ExpireStaleListingsAsync()
{
    var expired = await _dbContext.Listings
        .Where(l => l.Status == ListingStatus.Active && l.ExpiresAt < DateTime.UtcNow)
        .ToListAsync();

    foreach (var listing in expired)
        listing.Status = ListingStatus.Expired;

    if (expired.Count > 0)
        await _dbContext.SaveChangesAsync();
}
}