namespace ShopNest.Application.Common.Interfaces;

/// <summary>
/// Abstracts the Stripe SDK so the Application layer has no dependency
/// on Stripe.net. The Infrastructure layer provides the implementation.
/// </summary>
public interface IPaymentService
{
    Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(
        Guid orderId,
        decimal amount,
        string currency,
        CancellationToken ct = default);

    Task<PaymentRefundResult> RefundAsync(
        string chargeId,
        decimal amount,
        string? reason,
        CancellationToken ct = default);

    /// <summary>
    /// Validates the Stripe-Signature header and returns a strongly-typed
    /// event. Throws StripeException if the signature is invalid.
    /// Encapsulates all Stripe SDK types so they don't leak into Application.
    /// </summary>
    StripeWebhookEvent ParseWebhookEvent(
        string payload,
        string signature,
        string webhookSecret);
}

// ── Result types (Application-owned, no Stripe dependency) ─────────────────

public sealed record CreatePaymentIntentResult(
    string PaymentIntentId,
    string ClientSecret
);

public sealed record PaymentRefundResult(
    string RefundId,
    string Status
);

public sealed record StripeWebhookEvent(
    string Type,
    string ObjectId, // PaymentIntent ID or Charge ID
    string? ChargeId, // set on payment_intent.succeeded
    decimal? AmountRefunded, // set on charge.refunded
    bool IsFullRefund, // true if charge.refunded and Charge.Refunded == true
    string? FailureMessage // set on payment_intent.payment_failed
);