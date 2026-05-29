using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Payments.Commands.CreatePaymentIntent;
using ShopNest.Application.Features.Payments.Commands.HandleStripeWebhook;
using ShopNest.Application.Features.Payments.Queries.GetPaymentByOrderId;
using ShopNest.Application.Features.Payments.RefundPayment;

namespace ShopNest.API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/v1")]
public sealed class PaymentsController : BaseApiController
{
    // ── Customer endpoints ───────────────────────────────────────────

    /// POST /api/v1/payments/create-intent
    /// Returns a Stripe clientSecret the frontend uses with Stripe.js.
    [HttpPost("payments/create-intent")]
    [Authorize]
    public async Task<IActionResult> CreateIntent(
        [FromBody] CreateIntentRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new CreatePaymentIntentCommand(req.OrderId), ct));

    /// GET /api/v1/payments/{orderId}
    [HttpGet("payments/{orderId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByOrderId(
        Guid orderId, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetPaymentByOrderIdQuery(orderId), ct));

    // ── Stripe webhook ───────────────────────────────────────────────

    /// POST /api/v1/payments/webhook
    ///
    /// SECURITY: Stripe validates the raw body against the HMAC signature.
    /// Any middleware that buffers or re-encodes the body will break validation.
    /// We must:
    ///   1. Disable request size limit
    ///   2. Read body as raw UTF-8 BEFORE any other processing
    ///   3. ALWAYS return 200 OK — non-200 causes Stripe to retry
    [HttpPost("payments/webhook")]
    [AllowAnonymous]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Webhook(CancellationToken ct = default)
    {
        string payload;
        using (var reader = new StreamReader(
                   Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            payload = await reader.ReadToEndAsync(ct);
        }

        var signature = Request.Headers["Stripe-Signature"]
            .FirstOrDefault();

        if (string.IsNullOrEmpty(signature))
            return BadRequest("Missing Stripe-Signature header.");

        // Send to handler — result is intentionally ignored here.
        // Invalid signatures return Failure but we still return 200
        // so Stripe does not retry. Logging happens in LoggingBehavior.
        await Mediator.Send(
            new HandleStripeWebhookCommand(payload, signature), ct);

        return Ok();
    }

    // ── Admin endpoints ──────────────────────────────────────────────

    /// POST /api/v1/admin/payments/{id}/refund
    [HttpPost("admin/payments/{id:guid}/refund")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Refund(
        Guid id,
        [FromBody] RefundRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new RefundPaymentCommand(id, req.Amount, req.Reason), ct));
}

public sealed record CreateIntentRequest(Guid OrderId);

public sealed record RefundRequest(decimal Amount, string? Reason);