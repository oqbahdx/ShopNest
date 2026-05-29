using MediatR;
using ShopNest.Application.Features.DTOs;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(
    int Page = 1,
    int PageSize = 10,
    OrderStatus? Status = null
) : IRequest<Result<PagedResult<OrderListDto>>>;