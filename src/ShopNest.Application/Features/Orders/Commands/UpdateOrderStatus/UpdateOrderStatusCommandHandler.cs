using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IAppDbContext _db;

    public UpdateOrderStatusCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result> Handle(
        UpdateOrderStatusCommand cmd, CancellationToken ct)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == cmd.OrderId, ct);

        if (order is null)
            return Result.Failure("Order not found.", ErrorCodes.NOT_FOUND);

        try
        {
            // TransitionTo validates the state machine and throws
            // for illegal transitions
            order.TransitionTo(cmd.NewStatus);
        }
        catch (Exception ex) when (ex is InvalidOperationException or DomainException)
        {
            return Result.Failure(ex.Message, ErrorCodes.INVALID_ORDER_STATUS);
        }

        // Attach tracking number when shipping
        if (cmd.NewStatus == OrderStatus.Shipped
            && cmd.TrackingNumber is not null)
        {
            order.TrackingNumber = cmd.TrackingNumber;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}