using FluentValidation;

namespace ShopNest.Application.Features.Cart.Commands.UpdateCartItem;

public sealed class UpdateCartItemCommandValidator
    : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId)
            .NotEmpty().WithMessage("Cart item ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity cannot be negative.")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 per item.");
    }
}