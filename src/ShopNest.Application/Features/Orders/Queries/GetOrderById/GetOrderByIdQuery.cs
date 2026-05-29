using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(
    Guid Id,
    bool IsAdmin = false // Admins bypass the ownership check
) : IRequest<Result<OrderDto>>;