using MediatR;
using ShopNest.Application.Features.Payments.DTOs;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Payments.Commands.CreatePaymentIntent;

public sealed class CreatePaymentIntentCommandHandler
    : IRequestHandler<CreatePaymentIntentCommand, Result<PaymentIntentDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _paymentService;
    private const string Currency = "usd";

    public CreatePaymentIntentCommandHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IPaymentService paymentService)
    {
        _db = db;
        _currentUser = currentUser;
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentIntentDto>> Handle(
        CreatePaymentIntentCommand cmd, CancellationToken ct)
    {
        // 1. Load order
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == cmd.OrderId, ct);

        if (order is null)
            return Result<PaymentIntentDto>.Failure(
                "Order not found.", ErrorCodes.NOT_FOUND);

        // 2. Ownership check
        if (order.UserId != _currentUser.UserId)
            return Result<PaymentIntentDto>.Failure(
                "Access denied.", ErrorCodes.FORBIDDEN);

        // 3. Only Pending orders can be paid
        if (order.Status != OrderStatus.Pending)
            return Result<PaymentIntentDto>.Failure(
                "Payment can only be initiated for Pending orders.",
                ErrorCodes.INVALID_ORDER_STATUS);

        // 4. Idempotency: if a successful payment already exists, block
        var existing = await _db.Payments
            .FirstOrDefaultAsync(p => p.OrderId == cmd.OrderId, ct);

        if (existing?.Status == PaymentStatus.Succeeded)
            return Result<PaymentIntentDto>.Failure(
                "This order has already been paid.",
                ErrorCodes.CONFLICT);

        // 5. Create Stripe PaymentIntent
        var intentResult = await _paymentService.CreatePaymentIntentAsync(
            cmd.OrderId, order.TotalAmount, Currency, ct);

        // 6. Persist Payment entity (create or refresh on retry)
        if (existing is null)
        {
            var payment = Payment.Create(
                orderId: cmd.OrderId,
                stripePaymentIntentId: intentResult.PaymentIntentId,
                amount: order.TotalAmount,
                currency: Currency
            );
            _db.Payments.Add(payment);
        }
        else
        {
            // Refresh the intent ID so the client gets a fresh clientSecret
            existing.UpdatePaymentIntentId(intentResult.PaymentIntentId);
        }

        await _db.SaveChangesAsync(ct);

        return Result<PaymentIntentDto>.Success(new PaymentIntentDto(
            ClientSecret: intentResult.ClientSecret,
            Amount: order.TotalAmount,
            Currency: Currency
        ));
    }
}