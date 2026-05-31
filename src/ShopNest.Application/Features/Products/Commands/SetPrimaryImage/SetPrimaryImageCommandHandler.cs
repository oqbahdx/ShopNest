using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.SetPrimaryImage;

public sealed class SetPrimaryImageCommandHandler
    : IRequestHandler<SetPrimaryImageCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;
    public SetPrimaryImageCommandHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }
    public async Task<Result> Handle(
        SetPrimaryImageCommand cmd, CancellationToken ct)
    {
        // 1. Load all images for this product (needed to unset old primary)
        var images = await _db.ProductImages
            .Where(i => i.ProductId == cmd.ProductId)
            .ToListAsync(ct);
        if (!images.Any())
            return Result.Failure(
                "Product not found or has no images.", ErrorCodes.NOT_FOUND);
        // 2. Verify target image belongs to this product
        var target = images.FirstOrDefault(i => i.Id == cmd.ImageId);
        if (target is null)
            return Result.Failure(
                "Image not found or does not belong to this product.",
                ErrorCodes.NOT_FOUND);
        // 3. Swap primary — unset all, then set target
        foreach (var image in images)
            image.SetPrimary(image.Id == cmd.ImageId);
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Products.Prefix, ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result.Success();
    }
}