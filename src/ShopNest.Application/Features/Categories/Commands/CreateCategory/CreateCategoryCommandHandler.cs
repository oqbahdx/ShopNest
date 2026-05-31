using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Extensions;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;
    private const int MaxDepth = 3;
    public CreateCategoryCommandHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand cmd, CancellationToken ct)
    {
        // 1. Validate parent exists and depth constraint
        if (cmd.ParentCategoryId.HasValue)
        {
            var parent = await _db.Categories
                .Include(c => c.ParentCategory)
                .ThenInclude(p => p!.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == cmd.ParentCategoryId.Value, ct);
            if (parent is null)
                return Result<Guid>.Failure(
                    "Parent category not found.", ErrorCodes.NOT_FOUND);
            if (GetDepth(parent) >= MaxDepth)
                return Result<Guid>.Failure(
                    $"Maximum category depth of {MaxDepth} levels exceeded.",
                    ErrorCodes.CONFLICT);
        }

        // 2. Generate unique slug
        var baseSlug = cmd.Name.ToSlug();
        var slug = baseSlug;
        var counter = 1;
        while (await _db.Categories.AnyAsync(c => c.Slug == slug, ct))
            slug = $"{baseSlug}-{counter++}";
        // 3. Create and persist
        var category = Category.Create(
            name: cmd.Name,
            slug: slug,
            description: cmd.Description,
            imageUrl: cmd.ImageUrl,
            displayOrder: cmd.DisplayOrder,
            parentCategoryId: cmd.ParentCategoryId
        );
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result<Guid>.Success(category.Id);
    }

    private static int GetDepth(Category category)
    {
        var depth = 1;
        var current = category;
        while (current.ParentCategory is not null)
        {
            depth++;
            current = current.ParentCategory;
        }

        return depth;
    }
}