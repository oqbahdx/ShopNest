using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Wishlist.Queries.ValidateCoupon;

public sealed class ValidateCouponQueryHandler
    : IRequestHandler<ValidateCouponQuery, Result<CouponValidationDto>>
{
    private readonly IAppDbContext _db;

    public ValidateCouponQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<CouponValidationDto>> Handle(
        ValidateCouponQuery qry, CancellationToken ct)
    {
        var coupon = await _db.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Code == qry.Code.ToUpperInvariant(), ct);

        if (coupon is null)
            return Result<CouponValidationDto>.Success(new CouponValidationDto(
                IsValid: false,
                ErrorMessage: "Coupon not found.",
                DiscountAmount: 0,
                DiscountType: string.Empty));

        // Run the same validation chain as ApplyCouponCommandHandler
        if (!coupon.IsActive)
            return Invalid("This coupon is no longer active.");

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
            return Invalid("This coupon has expired.");

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
            return Invalid("This coupon has reached its usage limit.");

        if (coupon.MinimumOrderAmount.HasValue
            && qry.Subtotal < coupon.MinimumOrderAmount.Value)
        {
            return Invalid(
                $"Minimum order of {coupon.MinimumOrderAmount:C} required.");
        }

        var discount = coupon.CalculateDiscount(qry.Subtotal);

        return Result<CouponValidationDto>.Success(new CouponValidationDto(
            IsValid:        true,
            ErrorMessage:   null,
            DiscountAmount: discount,
            DiscountType:   coupon.DiscountType.ToString()
        ));
    }

    private static Result<CouponValidationDto> Invalid(string msg) =>
        Result<CouponValidationDto>.Success(new CouponValidationDto(
            IsValid: false, ErrorMessage: msg,
            DiscountAmount: 0, DiscountType: string.Empty));
}