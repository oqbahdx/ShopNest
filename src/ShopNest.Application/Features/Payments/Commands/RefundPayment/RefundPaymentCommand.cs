using MediatR;

namespace ShopNest.Application.Features.Payments.RefundPayment;

/// <summary>Admin-only. Initiates a partial or full refund via Stripe.</summary>
public sealed record RefundPaymentCommand(
    Guid    PaymentId,
    decimal Amount,
    string? Reason
) : IRequest<Result>;