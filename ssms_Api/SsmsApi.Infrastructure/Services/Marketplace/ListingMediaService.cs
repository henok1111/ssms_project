using Microsoft.EntityFrameworkCore;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces;
using SsmsApi.Application.Interfaces.Marketplace;
using SsmsApi.Domain.Entities.Marketplace;
using SsmsApi.Domain.Enums.Marketplace;
using SsmsApi.Infrastructure.Persistence;

namespace SsmsApi.Infrastructure.Services.Marketplace;

public class ListingMediaService : IListingMediaService
{
    private readonly SsmsDbContext _dbContext;
    private readonly IFileStorageService _fileStorage;

    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp" };
    private static readonly string[] AllowedAudioTypes = { "audio/mpeg", "audio/mp3", "audio/wav", "audio/ogg", "audio/webm", "audio/mp4" };

    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private const long MaxAudioSizeBytes = 10 * 1024 * 1024;
    private const int MaxMediaPerListing = 8; // slightly raised now that audio shares the same limit pool

    public ListingMediaService(SsmsDbContext dbContext, IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    public async Task<ListingMediaResponse> UploadAsync(
        Guid listingId, Guid sellerUserId, Stream fileStream, string fileName, string contentType)
    {
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == listingId)
            ?? throw new InvalidOperationException("Listing not found.");

        if (listing.SellerId != sellerUserId)
            throw new UnauthorizedAccessException("Only the listing's seller can add media.");

        ListingMediaType mediaType;
        if (AllowedImageTypes.Contains(contentType))
        {
            mediaType = ListingMediaType.Image;
            if (fileStream.Length > MaxImageSizeBytes)
                throw new InvalidOperationException("Image size cannot exceed 5MB.");
        }
        else if (AllowedAudioTypes.Contains(contentType))
        {
            mediaType = ListingMediaType.Audio;
            if (fileStream.Length > MaxAudioSizeBytes)
                throw new InvalidOperationException("Audio size cannot exceed 10MB.");
        }
        else
        {
            throw new InvalidOperationException("Only JPEG/PNG/WEBP images or MP3/WAV/OGG audio files are allowed.");
        }

        var currentCount = await _dbContext.ListingMedia.CountAsync(m => m.ListingId == listingId);
        if (currentCount >= MaxMediaPerListing)
            throw new InvalidOperationException($"A listing can have at most {MaxMediaPerListing} media files.");

        var fileUrl = await _fileStorage.SaveFileAsync(fileStream, fileName, contentType);

        var media = new ListingMedia
        {
            ListingId = listingId,
            FileUrl = fileUrl,
            MediaType = mediaType
        };

        _dbContext.ListingMedia.Add(media);
        await _dbContext.SaveChangesAsync();

        return new ListingMediaResponse { Id = media.Id, ListingId = media.ListingId, FileUrl = media.FileUrl, MediaType = media.MediaType };
    }

    public async Task<bool> DeleteAsync(Guid mediaId, Guid sellerUserId)
    {
        var media = await _dbContext.ListingMedia
            .Include(m => m.Listing)
            .FirstOrDefaultAsync(m => m.Id == mediaId);

        if (media is null || media.Listing.SellerId != sellerUserId) return false;

        media.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}