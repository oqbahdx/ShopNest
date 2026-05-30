using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Features.Cart.DTOs;
using ShopNest.Application.Features.Cart.Mappers;

namespace ShopNest.Application.Features.Cart.Commands.ApplyCoupon;

public sealed class ApplyCouponCommandHandler
    : IRequestHandler<ApplyCouponCommand, Result<CartDto>>
{
    private readonly IAppDbContext       _db;
    private readonly ICurrentUserService _currentUser;

    public ApplyCouponCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db          = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        ApplyCouponCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        // 1. Load cart (must have items to apply a coupon)
        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is null || !cart.Items.Any())
            return Result<CartDto>.Failure(
                "Cannot apply a coupon to an empty cart.",
                ErrorCodes.CONFLICT);

        // 2. Find coupon by code
        var coupon = await _db.Coupons
            .FirstOrDefaultAsync(
                c => c.Code == cmd.Code.ToUpperInvariant(), ct);

        if (coupon is null)
            return Result<CartDto>.Failure(
                "Coupon not found.", ErrorCodes.NOT_FOUND);

        // 3. Full validity chain
        if (!coupon.IsActive)
            return Result<CartDto>.Failure(
                "This coupon is no longer active.", ErrorCodes.CONFLICT);

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
            return Result<CartDto>.Failure(
                "This coupon has expired.", ErrorCodes.CONFLICT);

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
            return Result<CartDto>.Failure(
                "This coupon has reached its usage limit.", ErrorCodes.CONFLICT);

        if (coupon.MinimumOrderAmount.HasValue
            && cart.SubTotal < coupon.MinimumOrderAmount.Value)
        {
            return Result<CartDto>.Failure(
                $"Minimum order of {coupon.MinimumOrderAmount:C} required for this coupon.",
                ErrorCodes.CONFLICT);
        }

        // 4. One coupon per user — check if this user already used it
        var alreadyUsed = await _db.Orders
            .AnyAsync(o =>
                o.UserId        == userId &&
                o.CouponCode    == coupon.Code, ct);

        if (alreadyUsed)
            return Result<CartDto>.Failure(
                "You have already used this coupon.", ErrorCodes.CONFLICT);

        // 5. Apply and recalculate
        // CRITICAL: Always Recalculate after ApplyCoupon
        cart.ApplyCoupon(coupon);
        cart.Recalculate();
        await _db.SaveChangesAsync(ct);

        return Result<CartDto>.Success(CartMapper.ToDto(cart));
    }
}