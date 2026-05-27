using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateStock;

public sealed class UpdateStockCommandHandler
    : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IApplicationDbContext _db;
    public UpdateStockCommandHandler(IApplicationDbContext db) => _db = db;

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
        return Result.Success();
    }
}
