using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;
using ShopNest.Application.Features.Products.Queries.GetProducts;

namespace ShopNest.Application.Features.Products.Queries.GetProductBySlug;

public sealed class GetProductBySlugQueryHandler
    : IRequestHandler<GetProductBySlugQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _db;
    public GetProductBySlugQueryHandler(IApplicationDbContext db) => _db = db;
    public async Task<Result<ProductDto>> Handle(
        GetProductBySlugQuery qry, CancellationToken ct)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .FirstOrDefaultAsync(
                p => p.Slug == qry.Slug && p.IsActive, ct);
        if (product is null)
            return Result<ProductDto>.Failure(
                "Product not found.", ErrorCodes.NOT_FOUND);
        // Reuse mapper from GetProductByIdQueryHandler
        return Result<ProductDto>.Success(
            GetProductByIdQueryHandler.MapToDto(product));
    }
}