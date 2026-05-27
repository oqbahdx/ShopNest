using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed class GetAdminProductsQueryHandler
    : IRequestHandler<GetAdminProductsQuery, Result<PagedResult<AdminProductDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetAdminProductsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<AdminProductDto>>> Handle(
        GetAdminProductsQuery qry, CancellationToken ct)
    {
        // IgnoreQueryFilters() bypasses the global IsDeleted = false EF filter
        var q = _db.Products
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(qry.Search))
        {
            var term = qry.Search.Trim().ToLower();
            q = q.Where(p =>
                EF.Functions.Like(p.Name.ToLower(), $"%{term}%") ||
                EF.Functions.Like(p.SKU.ToLower(), $"%{term}%"));
        }

        if (qry.CategoryId.HasValue)
            q = q.Where(p => p.CategoryId == qry.CategoryId.Value);
        if (qry.IsActive.HasValue)
            q = q.Where(p => p.IsActive == qry.IsActive.Value);
        if (qry.IsDeleted.HasValue)
            q = q.Where(p => p.IsDeleted == qry.IsDeleted.Value);
        if (qry.LowStock == true)
            q = q.Where(p => p.StockQuantity <= p.LowStockThreshold);
        var totalCount = await q.CountAsync(ct);
        q = (qry.SortBy.ToLower(), qry.SortOrder.ToLower()) switch
        {
            ("name", "asc") => q.OrderBy(p => p.Name),
            ("name", _) => q.OrderByDescending(p => p.Name),
            ("price", "asc") => q.OrderBy(p => p.Price),
            ("price", _) => q.OrderByDescending(p => p.Price),
            ("stock", "asc") => q.OrderBy(p => p.StockQuantity),
            ("stock", _) => q.OrderByDescending(p => p.StockQuantity),
            ("createdat", "asc") => q.OrderBy(p => p.CreatedAt),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };
        var skip = (qry.Page - 1) * qry.PageSize;
        var products = await q.Skip(skip).Take(qry.PageSize).ToListAsync(ct);
        var items = products.Select(p => new AdminProductDto(
            Id: p.Id,
            Name: p.Name,
            Slug: p.Slug,
            Sku: p.SKU,
            Price: p.Price,
            CompareAtPrice: p.CompareAtPrice,
            CostPrice: p.CostPrice,
            StockQuantity: p.StockQuantity,
            LowStockThreshold: p.LowStockThreshold,
            IsInStock: p.StockQuantity > 0,
            IsFeatured: p.IsFeatured,
            IsActive: p.IsActive,
            IsDeleted: p.IsDeleted,
            CategoryName: p.Category.Name,
            AverageRating: p.AverageRating,
            ReviewCount: p.ReviewCount,
            PrimaryImageUrl: p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            CreatedAt: p.CreatedAt,
            UpdatedAt: p.UpdatedAt
        )).ToList();
        return Result<PagedResult<AdminProductDto>>.Success(
            PagedResult<AdminProductDto>.Create(
                items, qry.Page, qry.PageSize, totalCount));
    }
}