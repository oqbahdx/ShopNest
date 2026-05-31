using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;
    public DeleteCategoryCommandHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result> Handle(
        DeleteCategoryCommand cmd, CancellationToken ct)
    {
        // 1. Load category
        var category = await _db.Categories.FindAsync(
            new object[] { cmd.Id }, ct);
        if (category is null)
            return Result.Failure("Category not found.", ErrorCodes.NOT_FOUND);
        // 2. Guard: no active products
        var hasProducts = await _db.Products
            .AnyAsync(p => p.CategoryId == cmd.Id, ct);
        if (hasProducts)
            return Result.Failure(
                "Cannot delete a category that contains active products. " +
                "Reassign or remove them first.",
                ErrorCodes.CONFLICT);
        // 3. Guard: no child categories
        var hasChildren = await _db.Categories
            .AnyAsync(c => c.ParentCategoryId == cmd.Id, ct);
        if (hasChildren)
            return Result.Failure(
                "Cannot delete a category that has sub-categories. " +
                "Delete or reassign them first.",
                ErrorCodes.CONFLICT);
        // 4. Soft-delete (SaveChangesAsync intercepts via ISoftDeletable)
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result.Success();
    }
}