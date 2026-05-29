using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler
    : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IAppDbContext       _db;
    private readonly ICurrentUserService _currentUser;

    public CancelOrderCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db          = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        CancelOrderCommand cmd, CancellationToken ct)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == cmd.OrderId, ct);

        if (order is null)
            return Result.Failure("Order not found.", ErrorCodes.NOT_FOUND);

        // Ownership: customer can only cancel their own orders
        if (order.UserId != _currentUser.UserId)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        // Domain rule: only Pending or Confirmed orders can be cancelled
        if (!order.CanCancel())
            return Result.Failure(
                $"An order with status '{order.Status}' cannot be cancelled.",
                ErrorCodes.INVALID_ORDER_STATUS);

        order.TransitionTo(OrderStatus.Cancelled, cmd.Reason);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}