using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

/// <summary>
/// Returns top N featured products. Results are cached for 30 minutes.
/// Uses IMemoryCache in Phase 1; upgrades to Redis ICacheService in Phase 8.
/// </summary>
public sealed record GetFeaturedProductsQuery(int Top = 10)
    : IRequest<Result<List<ProductListDto>>>;