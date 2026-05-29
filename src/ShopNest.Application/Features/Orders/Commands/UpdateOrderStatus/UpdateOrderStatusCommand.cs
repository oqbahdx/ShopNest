using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Commands.UpdateOrderStatus;

/// <summary>
/// Admin-only. Advances the order state machine.
/// TrackingNumber is required when transitioning to Shipped.
/// </summary>
public sealed record UpdateOrderStatusCommand(
    Guid        OrderId,
    OrderStatus NewStatus,
    string?     TrackingNumber = null
) : IRequest<Result>;