using FluentValidation;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Coupons.Commands.CreateCoupon;

public sealed class CreateCouponCommandValidator
    : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.")
            .Matches(@"^[A-Z0-9-_]+$")
            .WithMessage("Code may only contain uppercase letters, numbers, hyphens and underscores.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than zero.");

        RuleFor(x => x.DiscountValue)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountType == DiscountType.Percentage)
            .WithMessage("Percentage discount cannot exceed 100%.");

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThan(0)
            .When(x => x.MinimumOrderAmount.HasValue)
            .WithMessage("Minimum order amount must be greater than zero.");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0)
            .When(x => x.UsageLimit.HasValue)
            .WithMessage("Usage limit must be greater than zero.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpiresAt.HasValue)
            .WithMessage("Expiry date must be in the future.");
    }
}