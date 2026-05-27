using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductListDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetProductsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<ProductListDto>>> Handle(
        GetProductsQuery qry, CancellationToken ct)
    {
        var q = _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();
        // --- Filters ---
        if (!string.IsNullOrWhiteSpace(qry.Search))
        {
            var term = qry.Search.Trim().ToLower();
            q = q.Where(p =>
                EF.Functions.Like(p.Name.ToLower(), $"%{term}%") ||
                (p.Description != null &&
                 EF.Functions.Like(p.Description.ToLower(), $"%{term}%")) ||
                EF.Functions.Like(p.SKU.ToLower(), $"%{term}%"));
        }

        if (qry.CategoryId.HasValue)
        {
            // Include products from sub-categories via recursive CTE
            var categoryIds = await GetCategoryIdsRecursiveAsync(
                qry.CategoryId.Value, ct);
            q = q.Where(p => categoryIds.Contains(p.CategoryId));
        }

        if (qry.MinPrice.HasValue)
            q = q.Where(p => p.Price >= qry.MinPrice.Value);
        if (qry.MaxPrice.HasValue)
            q = q.Where(p => p.Price <= qry.MaxPrice.Value);
        if (qry.InStock.HasValue)
            q = qry.InStock.Value
                ? q.Where(p => p.StockQuantity > 0)
                : q.Where(p => p.StockQuantity == 0);
        if (qry.Featured.HasValue)
            q = q.Where(p => p.IsFeatured == qry.Featured.Value);
        if (qry.MinRating.HasValue)
            q = q.Where(p => p.AverageRating >= qry.MinRating.Value);
        // --- Count before pagination ---
        var totalCount = await q.CountAsync(ct);
        // --- Sorting ---
        q = (qry.SortBy.ToLower(), qry.SortOrder.ToLower()) switch
        {
            ("name", "asc") => q.OrderBy(p => p.Name),
            ("name", _) => q.OrderByDescending(p => p.Name),
            ("price", "asc") => q.OrderBy(p => p.Price),
            ("price", _) => q.OrderByDescending(p => p.Price),
            ("rating", "asc") => q.OrderBy(p => p.AverageRating),
            ("rating", _) => q.OrderByDescending(p => p.AverageRating),
            ("createdat", "asc") => q.OrderBy(p => p.CreatedAt),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };
        // --- Pagination ---
        var skip = (qry.Page - 1) * qry.PageSize;
        var products = await q.Skip(skip).Take(qry.PageSize).ToListAsync(ct);
        var items = products.Select(p => new ProductListDto(
            Id: p.Id,
            Name: p.Name,
            Slug: p.Slug,
            Price: p.Price,
            CompareAtPrice: p.CompareAtPrice,
            PrimaryImageUrl: p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                             ?? p.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.ImageUrl,
            CategoryName: p.Category.Name,
            AverageRating: p.AverageRating,
            ReviewCount: p.ReviewCount,
            IsInStock: p.StockQuantity > 0,
            IsFeatured: p.IsFeatured
        )).ToList();
        return Result<PagedResult<ProductListDto>>.Success(
            PagedResult<ProductListDto>.Create(items, qry.Page, qry.PageSize, totalCount));
    }

    // Recursive walk to collect a category + all descendant IDs.
    // For Phase 1 this is fine; Phase 8 can upgrade to a raw CTE for large trees.
    private async Task<List<Guid>> GetCategoryIdsRecursiveAsync(
        Guid rootId, CancellationToken ct)
    {
        var allCategories = await _db.Categories
            .AsNoTracking()
            .Select(c => new { c.Id, c.ParentCategoryId })
            .ToListAsync(ct);
        var result = new List<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(rootId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);
            foreach (var child in allCategories.Where(c => c.ParentCategoryId == current))
                queue.Enqueue(child.Id);
        }

        return result;
    }
}