using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Helpers;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private const int MaxDepth = 3;
    private readonly IApplicationDbContext _db;

    public CreateCategoryCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parent = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value && c.IsActive, ct);
            if (parent is null)
                return Result<Guid>.Failure("Parent category not found.", ErrorCodes.NotFound);

            var depth = await GetDepthAsync(parent, ct) + 1;
            if (depth > MaxDepth)
                return Result<Guid>.Failure("Maximum category depth exceeded.", ErrorCodes.Conflict);
        }

        var slug = await GenerateUniqueCategorySlugAsync(request.Name, ct);
        var category = Category.Create(
            request.Name.Trim(),
            slug,
            request.Description,
            request.ImageUrl,
            request.DisplayOrder,
            request.ParentCategoryId);

        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(category.Id);
    }

    private async Task<int> GetDepthAsync(Category category, CancellationToken ct)
    {
        var depth = 1;
        var parentId = category.ParentCategoryId;

        while (parentId.HasValue)
        {
            var parent = await _db.Categories.FirstOrDefaultAsync(c => c.Id == parentId.Value, ct);
            if (parent is null)
                break;

            depth++;
            parentId = parent.ParentCategoryId;
        }

        return depth;
    }

    private async Task<string> GenerateUniqueCategorySlugAsync(string name, CancellationToken ct)
    {
        var baseSlug = SlugHelper.FromText(name);
        var slug = baseSlug;
        var counter = 1;

        while (await _db.Categories.AnyAsync(c => c.Slug == slug, ct))
            slug = $"{baseSlug}-{counter++}";

        return slug;
    }
}
