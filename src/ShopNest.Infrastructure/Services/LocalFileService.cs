using Microsoft.Extensions.Hosting;
using ShopNest.Application.Common.Interfaces;

namespace ShopNest.Infrastructure.Services;

public sealed class LocalFileService : IFileService
{
    private readonly IHostEnvironment _environment;

    public LocalFileService(IHostEnvironment environment) => _environment = environment;

    public Task DeleteAsync(string path, string? container, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
            return Task.CompletedTask;

        var relativePath = path.TrimStart('/', '\\');
        var fullPath = Path.IsPathRooted(relativePath)
            ? relativePath
            : Path.Combine(_environment.ContentRootPath, container ?? string.Empty, relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
