using MediatR;
using ShopNest.Application.Features.Payments.DTOs;

namespace ShopNest.Application.Features.Payments.Queries.GetPaymentByOrderId;

public sealed class GetPaymentByOrderIdQueryHandler
    : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetPaymentByOrderIdQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PaymentDto>> Handle(
        GetPaymentByOrderIdQuery qry, CancellationToken ct)
    {
        // Load payment and its associated order for ownership check
        var payment = await _db.Payments
            .AsNoTracking()
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == qry.OrderId, ct);

        if (payment is null)
            return Result<PaymentDto>.Failure(
                "Payment not found.", ErrorCodes.NOT_FOUND);

        // Customer can only see payment for their own order
        if (payment.Order.UserId != _currentUser.UserId)
            return Result<PaymentDto>.Failure(
                "Access denied.", ErrorCodes.FORBIDDEN);

        return Result<PaymentDto>.Success(new PaymentDto(
            Id: payment.Id,
            OrderId: payment.OrderId,
            Status: payment.Status,
            Amount: payment.Amount,
            Currency: payment.Currency,
            StripePaymentIntentId: payment.StripePaymentIntentId,
            StripeChargeId: payment.StripeChargeId,
            RefundedAmount: payment.RefundedAmount,
            FailureReason: payment.FailureReason,
            CreatedAt: payment.CreatedAt,
            UpdatedAt: payment.UpdatedAt
        ));
    }
}