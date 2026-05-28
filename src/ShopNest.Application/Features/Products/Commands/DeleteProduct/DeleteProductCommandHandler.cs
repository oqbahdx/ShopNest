using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private static readonly OrderStatus[] ActiveOrderStatuses =
    [
        OrderStatus.Pending,
        OrderStatus.Confirmed,
        OrderStatus.Processing,
        OrderStatus.Shipped
    ];

    private readonly IApplicationDbContext _db;

    public DeleteProductCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (product is null)
            return Result.Failure("Product not found.", ErrorCodes.NotFound);

        var hasActiveOrders = await _db.OrderItems.AnyAsync(oi =>
            oi.ProductId == request.Id &&
            _db.Orders.Any(o => o.Id == oi.OrderId && ActiveOrderStatuses.Contains(o.Status)), ct);

        if (hasActiveOrders)
            return Result.Failure("Product has active orders.", ErrorCodes.Conflict);

        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
