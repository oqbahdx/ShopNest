using MediatR;

namespace ShopNest.Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(
    Guid   OrderId,
    string Reason
) : IRequest<Result>;