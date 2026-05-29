using MediatR;

namespace ShopNest.Application.Features.Orders.Commands.RequestReturn;

public sealed record RequestReturnCommand(
    Guid   OrderId,
    string Reason
) : IRequest<Result>;