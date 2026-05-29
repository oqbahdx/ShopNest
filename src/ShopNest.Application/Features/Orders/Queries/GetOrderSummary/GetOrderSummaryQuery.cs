using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Queries.GetOrderSummary;

public sealed record GetOrderSummaryQuery : IRequest<Result<OrderSummaryDto>>;