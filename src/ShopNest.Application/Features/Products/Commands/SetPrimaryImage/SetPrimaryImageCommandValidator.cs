using FluentValidation;

namespace ShopNest.Application.Features.Products.Commands.SetPrimaryImage;

public sealed class SetPrimaryImageCommandValidator
    : AbstractValidator<SetPrimaryImageCommand>
{
    public SetPrimaryImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.ImageId)
            .NotEmpty().WithMessage("Image ID is required.");
    }
}