using MediatR;

namespace ShopNest.Application.Features.Coupons.Commands.UpdateCoupon;

public sealed class UpdateCouponCommandHandler
    : IRequestHandler<UpdateCouponCommand, Result>
{
    private readonly IAppDbContext _db;

    public UpdateCouponCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result> Handle(
        UpdateCouponCommand cmd, CancellationToken ct)
    {
        var coupon = await _db.Coupons.FindAsync(
            new object[] { cmd.Id }, ct);

        if (coupon is null)
            return Result.Failure("Coupon not found.", ErrorCodes.NOT_FOUND);

        coupon.Update(
            discountType: cmd.DiscountType,
            discountValue: cmd.DiscountValue,
            minimumOrderAmount: cmd.MinimumOrderAmount,
            maximumDiscountAmount: cmd.MaximumDiscountAmount,
            usageLimit: cmd.UsageLimit,
            expiresAt: cmd.ExpiresAt
        );

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}