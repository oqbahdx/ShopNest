using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Extensions;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;
    public UpdateCategoryCommandHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result> Handle(
        UpdateCategoryCommand cmd, CancellationToken ct)
    {
        // 1. Load category
        var category = await _db.Categories.FindAsync(
            new object[] { cmd.Id }, ct);
        if (category is null)
            return Result.Failure("Category not found.", ErrorCodes.NOT_FOUND);
        // 2. Validate proposed parent
        if (cmd.ParentCategoryId.HasValue)
        {
            var parentExists = await _db.Categories
                .AnyAsync(c => c.Id == cmd.ParentCategoryId.Value, ct);
            if (!parentExists)
                return Result.Failure("Parent category not found.", ErrorCodes.NOT_FOUND);
            // Guard: cannot set a descendant as parent (circular reference)
            var isDescendant = await IsDescendantAsync(
                cmd.ParentCategoryId.Value, cmd.Id, ct);
            if (isDescendant)
                return Result.Failure(
                    "Cannot set a descendant as the parent category.",
                    ErrorCodes.CONFLICT);
        }

        // 3. Regenerate slug only if name changed
        var slug = category.Slug;
        if (!category.Name.Equals(cmd.Name, StringComparison.OrdinalIgnoreCase))
        {
            var baseSlug = cmd.Name.ToSlug();
            slug = baseSlug;
            var counter = 1;
            while (await _db.Categories.AnyAsync(
                       c => c.Slug == slug && c.Id != cmd.Id, ct))
            {
                slug = $"{baseSlug}-{counter++}";
            }
        }

        // 4. Apply update via entity method
        category.Update(
            name: cmd.Name,
            slug: slug,
            description: cmd.Description,
            imageUrl: cmd.ImageUrl,
            displayOrder: cmd.DisplayOrder,
            parentCategoryId: cmd.ParentCategoryId
        );
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result.Success();
    }

    /// <summary>
    /// Walks up from <paramref name="candidateId"/> checking whether
    /// it ever reaches <paramref name="ancestorId"/> — detects circular refs.
    /// </summary>
    private async Task<bool> IsDescendantAsync(
        Guid candidateId, Guid ancestorId, CancellationToken ct)
    {
        var currentId = candidateId;
        while (true)
        {
            var cat = await _db.Categories.FindAsync(
                new object[] { currentId }, ct);
            if (cat?.ParentCategoryId is null) return false;
            if (cat.ParentCategoryId.Value == ancestorId) return true;
            currentId = cat.ParentCategoryId.Value;
        }
    }
}