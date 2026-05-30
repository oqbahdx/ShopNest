using FluentValidation;

namespace ShopNest.Application.Features.Wishlist.Commands.AddToWishlist;

public sealed class AddToWishlistCommandValidator
    : AbstractValidator<AddToWishlistCommand>
{
    public AddToWishlistCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}