using MediatR;

namespace ShopNest.Application.Features.Coupons.Commands.DeactivateCoupon;

public sealed class DeactivateCouponCommandHandler
    : IRequestHandler<DeactivateCouponCommand, Result>
{
    private readonly IAppDbContext _db;

    public DeactivateCouponCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result> Handle(
        DeactivateCouponCommand cmd, CancellationToken ct)
    {
        var coupon = await _db.Coupons.FindAsync(
            new object[] { cmd.Id }, ct);

        if (coupon is null)
            return Result.Failure("Coupon not found.", ErrorCodes.NOT_FOUND);

        coupon.Deactivate();  // sets IsActive = false
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}