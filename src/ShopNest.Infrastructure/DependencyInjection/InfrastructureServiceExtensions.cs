using Microsoft.Extensions.DependencyInjection;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Infrastructure.Services;

namespace ShopNest.Infrastructure;

public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Call from Program.cs: builder.Services.AddInfrastructure(builder.Environment);
    /// This extends the existing DI registration (Identity, EF, JWT, etc.)
    /// already set up in the Auth phase.
    /// </summary>
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services)
    {
        // Phase 1: local disk file service
        // Phase 8: swap for AzureBlobStorageFileService or S3FileService
        services.AddScoped<IFileService, LocalFileService>();
        services.AddHttpContextAccessor();
        services.AddMemoryCache(); // for category tree + featured products cache
        return services;
    }
}
