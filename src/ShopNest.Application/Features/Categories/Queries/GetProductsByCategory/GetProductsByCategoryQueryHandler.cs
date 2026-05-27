using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.GetCategories.Queries.GetProductsByCategory;

public sealed class GetProductsByCategoryQueryHandler
    : IRequestHandler<GetProductsByCategoryQuery, Result<PagedResult<ProductListDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetProductsByCategoryQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<ProductListDto>>> Handle(
        GetProductsByCategoryQuery qry, CancellationToken ct)
    {
        // 1. Verify category exists
        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == qry.CategoryId, ct);
        if (!categoryExists)
            return Result<PagedResult<ProductListDto>>.Failure(
                "Category not found.", ErrorCodes.NOT_FOUND);
        // 2. Collect all descendant category IDs (BFS)
        var allCategories = await _db.Categories
            .AsNoTracking()
            .Select(c => new { c.Id, c.ParentCategoryId })
            .ToListAsync(ct);
        var categoryIds = new List<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(qry.CategoryId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            categoryIds.Add(current);
            foreach (var child in allCategories
                         .Where(c => c.ParentCategoryId == current))
            {
                queue.Enqueue(child.Id);
            }
        }

        // 3. Query products in those categories
        var q = _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && categoryIds.Contains(p.CategoryId))
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();
        var totalCount = await q.CountAsync(ct);
        q = (qry.SortBy?.ToLower(), qry.SortOrder?.ToLower()) switch
        {
            ("name", "asc") => q.OrderBy(p => p.Name),
            ("name", _) => q.OrderByDescending(p => p.Name),
            ("price", "asc") => q.OrderBy(p => p.Price),
            ("price", _) => q.OrderByDescending(p => p.Price),
            ("rating", "asc") => q.OrderBy(p => p.AverageRating),
            ("rating", _) => q.OrderByDescending(p => p.AverageRating),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };
        var products = await q
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