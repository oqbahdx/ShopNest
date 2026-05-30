using FluentValidation;

namespace ShopNest.Application.Features.Cart.Commands.ApplyCoupon;

public sealed class ApplyCouponCommandValidator
    : AbstractValidator<ApplyCouponCommand>
{
    public ApplyCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .MaximumLength(50).WithMessage("Coupon code is too long.")
            .Matches(@"^[A-Z0-9-_]+$")
            .WithMessage("Coupon code may only contain uppercase letters, numbers, hyphens, and underscores.");
    }
}