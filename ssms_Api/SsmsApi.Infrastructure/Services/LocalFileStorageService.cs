using SsmsApi.Application.Interfaces;

namespace SsmsApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsFolder;
    private readonly string _baseUrl;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5154";

        if (!Directory.Exists(_uploadsFolder))
            Directory.CreateDirectory(_uploadsFolder);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(_uploadsFolder, safeFileName);

        using (var output = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(output);
        }

        return $"{_baseUrl}/uploads/{safeFileName}";
    }
}