using MediatR;
using CouponEntity = ShopNest.Domain.Entities.Coupon;

namespace ShopNest.Application.Features.Coupons.Commands.CreateCoupon;

public sealed class CreateCouponCommandHandler
    : IRequestHandler<CreateCouponCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;

    public CreateCouponCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(
        CreateCouponCommand cmd, CancellationToken ct)
    {
        // 1. Ensure code is unique (case-insensitive)
        var codeTaken = await _db.Coupons
            .AnyAsync(c => c.Code == cmd.Code.ToUpperInvariant(), ct);

        if (codeTaken)
            return Result<Guid>.Failure(
                "A coupon with this code already exists.",
                ErrorCodes.CONFLICT);

        // 2. Create coupon entity
        var coupon = CouponEntity.Create(
            code:                 cmd.Code.ToUpperInvariant(),
            discountType:         cmd.DiscountType,
            discountValue:        cmd.DiscountValue,
            minimumOrderAmount:   cmd.MinimumOrderAmount,
            maximumDiscountAmount:cmd.MaximumDiscountAmount,
            usageLimit:           cmd.UsageLimit,
            expiresAt:            cmd.ExpiresAt
        );

        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(coupon.Id);
    }
}