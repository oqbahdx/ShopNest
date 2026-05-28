using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Categories.DTOs;

namespace ShopNest.Application.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private const string CacheKey = "categories:tree";
    private readonly IApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public GetCategoriesQueryHandler(IApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        if (_cache.TryGetValue(CacheKey, out IReadOnlyList<CategoryDto>? cached) && cached is not null)
            return Result<IReadOnlyList<CategoryDto>>.Success(cached);

        var categories = await _db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

        var productCounts = await _db.Products
            .Where(p => p.IsActive)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count, ct);

        var rootCategories = categories
            .Where(c => c.ParentCategoryId is null)
            .ToList();

        var childrenByParent = categories
            .Where(c => c.ParentCategoryId.HasValue)
            .GroupBy(c => c.ParentCategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        int RecursiveCount(Guid categoryId)
        {
            var count = productCounts.GetValueOrDefault(categoryId);
            if (!childrenByParent.TryGetValue(categoryId, out var children))
                return count;

            return count + children.Sum(child => RecursiveCount(child.Id));
        }

        CategoryDto Build(Domain.Entities.Category category)
        {
            var children = childrenByParent.TryGetValue(category.Id, out var childList)
                ? childList.Select(Build).ToList()
                : [];

            return new CategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.Description,
                category.ImageUrl,
                category.DisplayOrder,
                category.IsActive,
                category.ParentCategoryId,
                RecursiveCount(category.Id),
                children);
        }

        var tree = rootCategories
            .Select(Build)
            .ToList();

        _cache.Set(CacheKey, tree, TimeSpan.FromMinutes(5));
        return Result<IReadOnlyList<CategoryDto>>.Success(tree);
    }
}
