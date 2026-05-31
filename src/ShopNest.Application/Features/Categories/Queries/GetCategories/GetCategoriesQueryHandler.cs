using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.GetCategories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCategoriesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<CategoryDto>>> Handle(
        GetCategoriesQuery _, CancellationToken ct)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
        var productCounts = await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CategoryId, g => g.Count, ct);

        var tree = categories
            .Where(c => c.ParentCategoryId is null)
            .Select(c => MapToDto(c, categories, productCounts))
            .ToList();
        return Result<List<CategoryDto>>.Success(tree);
    }

    private static CategoryDto MapToDto(
        Category category,
        List<Category> all,
        Dictionary<Guid, int> productCounts)
    {
        var children = all
            .Where(c => c.ParentCategoryId == category.Id)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => MapToDto(c, all, productCounts))
            .ToList();
        var directCount = productCounts.GetValueOrDefault(category.Id, 0);
        var childrenCount = children.Sum(c => c.ProductCount);
        return new CategoryDto(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug,
            Description: category.Description,
            ImageUrl: category.ImageUrl,
            DisplayOrder: category.DisplayOrder,
            ProductCount: directCount + childrenCount,
            SubCategories: children
        );
    }
}
