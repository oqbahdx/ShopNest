using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

/// <summary>
/// Returns top N featured products. Results are cached for 30 minutes.
/// </summary>
public sealed record GetFeaturedProductsQuery(int Top = 10)
    : IRequest<Result<List<ProductListDto>>>, ICacheableQuery
{
    public string CacheKey => $"{CacheKeys.Products.Featured}:{Top}";
    public TimeSpan Ttl => TimeSpan.FromMinutes(30);
}