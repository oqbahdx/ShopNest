using MediatR;
using ShopNest.Application.Features.Payments.DTOs;

namespace ShopNest.Application.Features.Payments.Commands.CreatePaymentIntent;

public sealed record CreatePaymentIntentCommand(Guid OrderId)
    : IRequest<Result<PaymentIntentDto>>;