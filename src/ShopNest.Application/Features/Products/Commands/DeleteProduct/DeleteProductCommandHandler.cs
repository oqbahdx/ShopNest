using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IApplicationDbContext _db;
    public DeleteProductCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(
        DeleteProductCommand cmd, CancellationToken ct)
    {
        // 1. Load product
        var product = await _db.Products.FindAsync(
            new object[] { cmd.Id }, ct);
        if (product is null)
            return Result.Failure("Product not found.", ErrorCodes.NOT_FOUND);
        // 2. Guard: no active orders reference this product
        var activeStatuses = new[]
        {
            OrderStatus.Pending,
            OrderStatus.Confirmed,
            OrderStatus.Processing,
            OrderStatus.Shipped
        };
        var hasActiveOrders = await _db.OrderItems
            .AnyAsync(oi =>
                oi.ProductId == cmd.Id &&
                activeStatuses.Contains(oi.Order.Status), ct);
        if (hasActiveOrders)
            return Result.Failure(
                "Cannot delete a product referenced by active orders.",
                ErrorCodes.CONFLICT);
        // 3. Soft-delete via EF + ISoftDeletable (SaveChangesAsync intercepts)
        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}