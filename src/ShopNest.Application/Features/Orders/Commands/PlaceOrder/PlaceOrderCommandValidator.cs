using FluentValidation;

namespace ShopNest.Application.Features.Orders.Commands;

public sealed class PlaceOrderCommandValidator
    : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.ShippingFullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ShippingLine1)
            .NotEmpty().WithMessage("Address line 1 is required.")
            .MaximumLength(200);

        RuleFor(x => x.ShippingCity)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100);

        RuleFor(x => x.ShippingState)
            .NotEmpty().WithMessage("State / province is required.")
            .MaximumLength(100);

        RuleFor(x => x.ShippingPostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .MaximumLength(20);

        RuleFor(x => x.ShippingCountry)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);
    }
}