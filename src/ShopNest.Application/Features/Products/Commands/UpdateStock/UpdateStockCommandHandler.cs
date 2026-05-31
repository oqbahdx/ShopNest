using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateStock;

public sealed class UpdateStockCommandHandler
    : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;
    public UpdateStockCommandHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result> Handle(
        UpdateStockCommand cmd, CancellationToken ct)
    {
        // 1. Load product
        var product = await _db.Products.FindAsync(
            new object[] { cmd.ProductId }, ct);
        if (product is null)
            return Result.Failure("Product not found.", ErrorCodes.NOT_FOUND);
        // 2. Apply manual stock adjustment via entity method
        product.SetStock(cmd.NewQuantity);
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Products.Prefix, ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result.Success();
    }
}
