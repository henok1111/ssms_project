using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Infrastructure.Persistence;
using SsmsApi.Domain.Enums.Marketplace;
namespace SsmsApi.Infrastructure.Services.Marketplace;

public class SavedListingService : ISavedListingService
{
    private readonly SsmsDbContext _dbContext;

    public SavedListingService(SsmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

  public async Task<IReadOnlyList<ListingResponse>> GetMySavedAsync(Guid userId)
{
    var saved = await _dbContext.SavedListings
        .Include(s => s.Listing).ThenInclude(l => l.Seller)
        .Include(s => s.Listing).ThenInclude(l => l.Category)
        .Include(s => s.Listing).ThenInclude(l => l.Media)
        .Where(s => s.UserId == userId)
        .OrderByDescending(s => s.CreatedAt)
        .ToListAsync();

    return saved.Select(s => new ListingResponse
    {
        Id = s.Listing.Id,
        SellerId = s.Listing.SellerId,
        SellerName = s.Listing.Seller.FullName,
        CategoryId = s.Listing.CategoryId,
        CategoryName = s.Listing.Category.Name,
        Title = s.Listing.Title,
        Description = s.Listing.Description,
        Price = s.Listing.Price,
        Condition = s.Listing.Condition,
        Location = s.Listing.Location,
        Status = s.Listing.Status,
        ImageUrls = s.Listing.Media
            .Where(m => m.MediaType == ListingMediaType.Image)
            .Select(m => m.FileUrl)
            .ToList(),
        AudioUrls = s.Listing.Media
            .Where(m => m.MediaType == ListingMediaType.Audio)
            .Select(m => m.FileUrl)
            .ToList(),
        ViewCount = s.Listing.ViewCount,
        CreatedAt = s.Listing.CreatedAt
    }).ToList();
}
    public async Task<bool> SaveAsync(Guid userId, Guid listingId)
    {
        var alreadySaved = await _dbContext.SavedListings
            .AnyAsync(s => s.UserId == userId && s.ListingId == listingId);
        if (alreadySaved) return true; // idempotent — saving twice is a no-op, not an error

        var listingExists = await _dbContext.Listings.AnyAsync(l => l.Id == listingId);
        if (!listingExists) return false;

        _dbContext.SavedListings.Add(new SavedListing { UserId = userId, ListingId = listingId });
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnsaveAsync(Guid userId, Guid listingId)
    {
        var saved = await _dbContext.SavedListings
            .FirstOrDefaultAsync(s => s.UserId == userId && s.ListingId == listingId);

        if (saved is null) return false;

        _dbContext.SavedListings.Remove(saved);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}