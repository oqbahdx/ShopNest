using MediatR;
using ShopNest.Application.Features.Payments.DTOs;

namespace ShopNest.Application.Features.Payments.Queries.GetPaymentByOrderId;

public sealed record GetPaymentByOrderIdQuery(Guid OrderId)
    : IRequest<Result<PaymentDto>>;