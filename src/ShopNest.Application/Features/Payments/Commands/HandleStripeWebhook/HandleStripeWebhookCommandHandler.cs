using MediatR;
using Microsoft.Extensions.Options;
using ShopNest.Application.Common.Settings;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Payments.Commands.HandleStripeWebhook;


public sealed class HandleStripeWebhookCommandHandler
    : IRequestHandler<HandleStripeWebhookCommand, Result>
{
    private readonly IAppDbContext             _db;
    private readonly IPaymentService           _paymentService;
    private readonly IOptions<StripeSettings>  _settings;

    public HandleStripeWebhookCommandHandler(
        IAppDbContext            db,
        IPaymentService          paymentService,
        IOptions<StripeSettings> settings)
    {
        _db             = db;
        _paymentService = paymentService;
        _settings       = settings;
    }

    public async Task<Result> Handle(
        HandleStripeWebhookCommand cmd, CancellationToken ct)
    {
        // ── Signature validation ──────────────────────────────────
        // ParseWebhookEvent throws StripeException for invalid signatures.
        // The controller always returns 200 to Stripe regardless, but this
        // prevents processing spoofed events.
        StripeWebhookEvent webhookEvent;
        try
        {
            webhookEvent = _paymentService.ParseWebhookEvent(
                cmd.Payload,
                cmd.StripeSignature,
                _settings.Value.WebhookSecret);
        }
        catch
        {
            // Invalid signature — log and discard, do not process
            return Result.Failure(
                "Invalid webhook signature.", "INVALID_SIGNATURE");
        }

        // ── Event dispatch ────────────────────────────────────────
        switch (webhookEvent.Type)
        {
            case "payment_intent.succeeded":
                await HandlePaymentSucceededAsync(webhookEvent, ct);
                break;

            case "payment_intent.payment_failed":
                await HandlePaymentFailedAsync(webhookEvent, ct);
                break;

            case "charge.refunded":
                await HandleChargeRefundedAsync(webhookEvent, ct);
                break;

            default:
                // All unhandled events: log and return 200 immediately.
                // NEVER return non-200 to Stripe — it will keep retrying.
                break;
        }

        return Result.Success();
    }

    // ── Event handlers ────────────────────────────────────────────

    private async Task HandlePaymentSucceededAsync(
        StripeWebhookEvent ev, CancellationToken ct)
    {
        var payment = await _db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(
                p => p.StripePaymentIntentId == ev.ObjectId, ct);

        if (payment is null) return;

        // IDEMPOTENCY: Stripe may deliver the same event more than once.
        // If we already processed it, skip silently.
        if (payment.Status == PaymentStatus.Succeeded) return;

        payment.MarkSucceeded(ev.ChargeId ?? string.Empty);
        payment.Order.TransitionTo(OrderStatus.Confirmed);

        await _db.SaveChangesAsync(ct);
    }

    private async Task HandlePaymentFailedAsync(
        StripeWebhookEvent ev, CancellationToken ct)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(
                p => p.StripePaymentIntentId == ev.ObjectId, ct);

        if (payment is null) return;

        // IDEMPOTENCY
        if (payment.Status == PaymentStatus.Failed) return;

        payment.MarkFailed(ev.FailureMessage ?? "Payment declined.");
        // Order intentionally stays Pending — customer can retry.

        await _db.SaveChangesAsync(ct);
    }

    private async Task HandleChargeRefundedAsync(
        StripeWebhookEvent ev, CancellationToken ct)
    {
        var payment = await _db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(
                p => p.StripeChargeId == ev.ObjectId, ct);

        if (payment is null) return;

        var refundAmount = ev.AmountRefunded ?? 0m;
        payment.ApplyRefund(refundAmount, ev.ObjectId);

        if (ev.IsFullRefund)
        {
            // Full refund → cancel the order and notify
            try
            {
                payment.Order.TransitionTo(OrderStatus.Cancelled);
            }
            catch
            {
                // Order may already be Cancelled — ignore duplicate transition
            }
        }

        await _db.SaveChangesAsync(ct);
    }
}
