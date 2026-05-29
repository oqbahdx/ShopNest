using MediatR;
using ShopNest.Application.Features.DTOs;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Queries.GetAdminOrders;

public sealed record GetAdminOrdersQuery(
    int Page = 1,
    int PageSize = 20,
    OrderStatus? Status = null,
    Guid? UserId = null,
    DateTime? From = null,
    DateTime? To = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string SortBy = "createdAt",
    string SortOrder = "desc"
) : IRequest<Result<PagedResult<AdminOrderDto>>>;