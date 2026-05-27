using FluentValidation;

namespace ShopNest.Application.Features.Products.Commands.UpdateStock;

public sealed class UpdateStockCommandValidator
    : AbstractValidator<UpdateStockCommand>
{
    public UpdateStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.NewQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative.");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("A reason for the stock adjustment is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}