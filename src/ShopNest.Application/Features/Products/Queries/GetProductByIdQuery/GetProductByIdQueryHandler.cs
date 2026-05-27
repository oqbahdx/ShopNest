using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _db;
    public GetProductByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery qry, CancellationToken ct)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Id == qry.Id && p.IsActive, ct);
        if (product is null)
            return Result<ProductDto>.Failure(
                "Product not found.", ErrorCodes.NOT_FOUND);
        return Result<ProductDto>.Success(MapToDto(product));
    }

    internal static ProductDto MapToDto(Domain.Entities.Product p) => new(
        Id: p.Id,
        Name: p.Name,
        Slug: p.Slug,
        Description: p.Description,
        ShortDescription: p.ShortDescription,
        Sku: p.SKU,
        Price: p.Price,
        CompareAtPrice: p.CompareAtPrice,
        Weight: p.Weight,
        IsFeatured: p.IsFeatured,
        IsActive: p.IsActive,
        AverageRating: p.AverageRating,
        ReviewCount: p.ReviewCount,
        IsInStock: p.StockQuantity > 0,
        Category: new ProductCategoryDto(
            p.Category.Id,
            p.Category.Name,
            p.Category.Slug),
        Images: p.Images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ProductImageDto(
                i.Id, i.ImageUrl, i.AltText, i.DisplayOrder, i.IsPrimary))
            .ToList()
    );
}