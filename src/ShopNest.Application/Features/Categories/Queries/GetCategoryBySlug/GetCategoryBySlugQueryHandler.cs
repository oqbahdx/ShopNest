using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.GetCategories.Queries.GetCategoryBySlug;

public sealed class GetCategoryBySlugQueryHandler
    : IRequestHandler<GetCategoryBySlugQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _db;
    public GetCategoryBySlugQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryBySlugQuery qry, CancellationToken ct)
    {
        var category = await _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == qry.Slug, ct);
        if (category is null)
            return Result<CategoryDto>.Failure(
                "Category not found.", ErrorCodes.NOT_FOUND);
        // Load immediate children
        var children = await _db.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == category.Id)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct);
        // Get product count for this category only (direct products)
        var productCount = await _db.Products
            .AsNoTracking()
            .CountAsync(p => p.CategoryId == category.Id && p.IsActive, ct);
        var dto = new CategoryDto(
            Id: category.Id,
            Name: category.Name,
            Slug: category.Slug,
            Description: category.Description,
            ImageUrl: category.ImageUrl,
            DisplayOrder: category.DisplayOrder,
            ProductCount: productCount,
            SubCategories: children.Select(c => new CategoryDto(
                c.Id, c.Name, c.Slug, c.Description, c.ImageUrl,
                c.DisplayOrder, 0, [])).ToList()
        );
        return Result<CategoryDto>.Success(dto);
    }
}