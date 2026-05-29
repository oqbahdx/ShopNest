using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Commands.RequestReturn;

public sealed class RequestReturnCommandHandler
    : IRequestHandler<RequestReturnCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RequestReturnCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        RequestReturnCommand cmd, CancellationToken ct)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == cmd.OrderId, ct);

        if (order is null)
            return Result.Failure("Order not found.", ErrorCodes.NOT_FOUND);

        if (order.UserId != _currentUser.UserId)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        // CanReturn: status must be Delivered AND within 30-day window
        if (!order.CanReturn())
            return Result.Failure(
                "This order is not eligible for return. " +
                "Returns are accepted within 30 days of delivery.",
                ErrorCodes.INVALID_ORDER_STATUS);

        order.TransitionTo(OrderStatus.ReturnRequested, cmd.Reason);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}