using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Payments.RefundPayment;

public sealed class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly IPaymentService _paymentService;

    public RefundPaymentCommandHandler(
        IAppDbContext db, IPaymentService paymentService)
    {
        _db = db;
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(
        RefundPaymentCommand cmd, CancellationToken ct)
    {
        // 1. Load payment with order
        var payment = await _db.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == cmd.PaymentId, ct);

        if (payment is null)
            return Result.Failure("Payment not found.", ErrorCodes.NOT_FOUND);

        // 2. Can only refund successful payments
        if (payment.Status != PaymentStatus.Succeeded)
            return Result.Failure(
                "Only successful payments can be refunded.",
                ErrorCodes.CONFLICT);

        // 3. Guard: refund cannot exceed original amount minus any previous refunds
        var refundable = payment.Amount - payment.RefundedAmount;
        if (cmd.Amount > refundable)
            return Result.Failure(
                $"Maximum refundable amount is {refundable:C}.",
                ErrorCodes.CONFLICT);

        if (string.IsNullOrEmpty(payment.StripeChargeId))
            return Result.Failure(
                "Cannot refund — no Stripe charge ID on record.",
                ErrorCodes.CONFLICT);

        // 4. Call Stripe Refunds API
        var refundResult = await _paymentService.RefundAsync(
            payment.StripeChargeId, cmd.Amount, cmd.Reason, ct);

        // 5. Update local state
        payment.ApplyRefund(cmd.Amount, refundResult.RefundId);

        // Full refund → transition order to Cancelled
        if (cmd.Amount >= refundable)
        {
            try
            {
                payment.Order.TransitionTo(OrderStatus.Cancelled);
            }
            catch
            {
                /* May already be Cancelled — safe to ignore */
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}