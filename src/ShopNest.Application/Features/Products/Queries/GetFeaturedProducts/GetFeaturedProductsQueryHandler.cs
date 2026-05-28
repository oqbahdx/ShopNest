using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed class GetFeaturedProductsQueryHandler
    : IRequestHandler<GetFeaturedProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public GetFeaturedProductsQueryHandler(IApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(GetFeaturedProductsQuery request, CancellationToken ct)
    {
        var top = Math.Clamp(request.Top, 1, 100);
        var cacheKey = $"products:featured:{top}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<ProductDto>? cached) && cached is not null)
            return Result<IReadOnlyList<ProductDto>>.Success(cached);

        var products = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(top)
            .Select(p => ProductMappings.ToDto(p))
            .ToListAsync(ct);

        _cache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
        return Result<IReadOnlyList<ProductDto>>.Success(products);
    }
}
