using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Queries;

/// <summary>Human-readable order number lookup, e.g. ORD-20240315-A4X9.</summary>
public sealed record GetOrderByNumberQuery(string OrderNumber)
    : IRequest<Result<OrderDto>>;