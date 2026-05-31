using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed class GetFeaturedProductsQueryHandler
    : IRequestHandler<GetFeaturedProductsQuery, Result<List<ProductListDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetFeaturedProductsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<Result<List<ProductListDto>>> Handle(
        GetFeaturedProductsQuery qry, CancellationToken ct)
    {
        var products = await _db.Products
            .AsNoTracking()
            .Where(p => p.IsFeatured && p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .OrderByDescending(p => p.AverageRating)
            .Take(qry.Top)
            .ToListAsync(ct);
        var result = products.Select(p => new ProductListDto(
            Id: p.Id,
            Name: p.Name,
            Slug: p.Slug,
            Price: p.Price,
            CompareAtPrice: p.CompareAtPrice,
            PrimaryImageUrl: p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            CategoryName: p.Category.Name,
            AverageRating: p.AverageRating,
            ReviewCount: p.ReviewCount,
            IsInStock: p.StockQuantity > 0,
            IsFeatured: true
        )).ToList();
        return Result<List<ProductListDto>>.Success(result);
    }
}
