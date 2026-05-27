using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ShopNest.Application.Common.Interfaces;

namespace ShopNest.Infrastructure.Services;

/// <summary>
/// Phase 1 stub — stores files on local disk under wwwroot/uploads/{container}.
/// Swap for AzureBlobStorageFileService / S3FileService in Phase 8.
/// </summary>
public sealed class LocalFileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContext;
    public LocalFileService(
        IWebHostEnvironment env,
        IHttpContextAccessor httpContext)
    {
        _env         = env;
        _httpContext = httpContext;
    }
    public async Task<FileUploadResult> UploadAsync(
        IFormFile file, string container, CancellationToken ct = default)
    {
        var uploadsDir = Path.Combine(
            _env.WebRootPath, "uploads", container);
        Directory.CreateDirectory(uploadsDir);
        var ext      = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);
        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream, ct);
        var request = _httpContext.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var url     = $"{baseUrl}/uploads/{container}/{fileName}";
        return new FileUploadResult(url, fileName, file.Length);
    }



    public Task DeleteAsync(
        string fileUrl, string container, CancellationToken ct = default)
    {
        try
        {
            // Extract filename from URL and resolve the physical path
            var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
            var filePath = Path.Combine(
                _env.WebRootPath, "uploads", container, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch
        {
            // Swallow — best-effort cleanup; don't fail the business operation
        }
        return Task.CompletedTask;
    }
}