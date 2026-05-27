using FluentValidation;

namespace ShopNest.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandValidator
    : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}