using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Payments.DTOs;

public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    PaymentStatus Status,
    decimal Amount,
    string Currency,
    string? StripePaymentIntentId,
    string? StripeChargeId,
    decimal RefundedAmount,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime UpdatedAt
);