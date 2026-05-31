using Microsoft.Extensions.DependencyInjection;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Infrastructure.Services;

namespace ShopNest.Infrastructure;

public static class InfrastructureServiceExtensions
{
	public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<IFileService, CloudflareR2FileService>();
		services.AddScoped<ICacheService, RedisCacheService>();
		return services;
	}
}
