using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.SearchProducts;

public sealed class SearchProductsQueryHandler
    : IRequestHandler<SearchProductsQuery, Result<PagedResult<ProductListDto>>>
{
    private readonly IApplicationDbContext _db;
    public SearchProductsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<ProductListDto>>> Handle(
        SearchProductsQuery qry, CancellationToken ct)
    {
        var term = qry.Search.Trim();
        var q = _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p =>
                EF.Functions.Contains(p.Name, $"\"{term}*\"") ||
                EF.Functions.Contains(p.SKU, $"\"{term}*\"") ||
                (p.Description != null &&
                 EF.Functions.Contains(p.Description, $"\"{term}*\"")));
        var totalCount = await q.CountAsync(ct);
        var products = await q
            .OrderBy(p =>
                EF.Functions.Contains(p.Name, $"\"{term}*\"") ? 0 :
                EF.Functions.Contains(p.SKU, $"\"{term}*\"") ? 1 : 2)
            .ThenByDescending(p => p.AverageRating)
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .ToListAsync(ct);
        var items = products.Select(p => new ProductListDto(
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
            IsFeatured: p.IsFeatured
        )).ToList();
        return Result<PagedResult<ProductListDto>>.Success(
            PagedResult<ProductListDto>.Create(
                items, qry.Page, qry.PageSize, totalCount));
    }
}