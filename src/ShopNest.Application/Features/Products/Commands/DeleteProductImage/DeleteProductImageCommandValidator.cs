using FluentValidation;

namespace ShopNest.Application.Features.Products.Commands.DeleteProductImage;

public sealed class DeleteProductImageCommandValidator
    : AbstractValidator<DeleteProductImageCommand>
{
    public DeleteProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.ImageId)
            .NotEmpty().WithMessage("Image ID is required.");
    }
}