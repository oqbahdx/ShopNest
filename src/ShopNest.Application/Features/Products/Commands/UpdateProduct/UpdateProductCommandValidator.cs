using FluentValidation;

namespace ShopNest.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cost price must be zero or greater.");
        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(0)
            .When(x => x.CompareAtPrice.HasValue)
            .WithMessage("Compare-at price must be greater than zero.");
        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Low stock threshold cannot be negative.");
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}
