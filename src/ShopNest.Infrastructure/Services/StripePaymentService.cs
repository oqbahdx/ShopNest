using Microsoft.Extensions.Options;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Settings;
using Stripe;

namespace ShopNest.Infrastructure.Services;

/// <summary>
/// Production implementation of IPaymentService using Stripe.net SDK.
/// All Stripe types are contained here — Application layer sees only
/// the Application-owned result types defined in IPaymentService.cs.
/// </summary>
public sealed class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;

    public StripePaymentService(IOptions<StripeSettings> settings)
    {
        _settings = settings.Value;
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(
        Guid orderId, decimal amount, string currency,
        CancellationToken ct = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            // Stripe amounts are in the smallest currency unit (cents for USD)
            Amount = (long)(amount * 100),
            Currency = currency.ToLowerInvariant(),
            Metadata = new Dictionary<string, string>
            {
                ["orderId"] = orderId.ToString()
            },
            AutomaticPaymentMethods =
                new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(
            options, cancellationToken: ct);

        return new CreatePaymentIntentResult(
            intent.Id, intent.ClientSecret);
    }

    public async Task<PaymentRefundResult> RefundAsync(
        string chargeId, decimal amount, string? reason,
        CancellationToken ct = default)
    {
        var options = new RefundCreateOptions
        {
            Charge = chargeId,
            Amount = (long)(amount * 100),
            Reason = reason
        };

        var service = new RefundService();
        var refund = await service.CreateAsync(
            options, cancellationToken: ct);

        return new PaymentRefundResult(refund.Id, refund.Status);
    }

    public StripeWebhookEvent ParseWebhookEvent(
        string payload, string signature, string webhookSecret)
    {
        // Throws StripeException if signature is invalid
        var stripeEvent = EventUtility.ConstructEvent(
            payload, signature, webhookSecret);

        return stripeEvent.Type switch
        {
            "payment_intent.succeeded" => ParsePiSucceeded(stripeEvent),
            "payment_intent.payment_failed" => ParsePiFailed(stripeEvent),
            "charge.refunded" => ParseChargeRefunded(stripeEvent),
            _ => new StripeWebhookEvent(
                stripeEvent.Type, string.Empty,
                null, null, false, null)
        };
    }

    // ── Private parsers ──────────────────────────────────────────────

    private static StripeWebhookEvent ParsePiSucceeded(Event e)
    {
        var pi = e.Data.Object as PaymentIntent;
        return new StripeWebhookEvent(
            Type: e.Type,
            ObjectId: pi?.Id ?? string.Empty,
            ChargeId: pi?.LatestChargeId,
            AmountRefunded: null,
            IsFullRefund: false,
            FailureMessage: null);
    }

    private static StripeWebhookEvent ParsePiFailed(Event e)
    {
        var pi = e.Data.Object as PaymentIntent;
        return new StripeWebhookEvent(
            Type: e.Type,
            ObjectId: pi?.Id ?? string.Empty,
            ChargeId: null,
            AmountRefunded: null,
            IsFullRefund: false,
            FailureMessage: pi?.LastPaymentError?.Message);
    }

    private static StripeWebhookEvent ParseChargeRefunded(Event e)
    {
        var charge = e.Data.Object as Charge;
        return new StripeWebhookEvent(
            Type: e.Type,
            ObjectId: charge?.Id ?? string.Empty,
            ChargeId: charge?.Id,
            AmountRefunded: charge?.AmountRefunded / 100m,
            IsFullRefund: charge?.Refunded ?? false,
            FailureMessage: null);
    }
}
