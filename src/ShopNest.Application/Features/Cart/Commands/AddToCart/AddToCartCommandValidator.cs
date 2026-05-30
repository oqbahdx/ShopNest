using FluentValidation;

namespace ShopNest.Application.Features.Cart.Commands.AddToCart;

public sealed class AddToCartCommandValidator
    : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 per item.");
    }
}