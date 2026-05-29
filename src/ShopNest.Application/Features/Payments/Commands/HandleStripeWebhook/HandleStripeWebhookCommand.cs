using MediatR;

namespace ShopNest.Application.Features.Payments.Commands.HandleStripeWebhook;

/// <summary>
/// No validator — the raw payload is validated by Stripe's HMAC
/// signature check inside the handler. Invalid signatures are rejected
/// before any DB work occurs.
/// </summary>
public sealed record HandleStripeWebhookCommand(
    string Payload,
    string StripeSignature
) : IRequest<Result>;