using FluentValidation;

namespace ShopNest.Application.Features.Users.Commands.AddAddress;

public sealed class AddAddressCommandValidator
    : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Line1)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty().MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Phone)
            .Matches(@"^+?[1-9]d{6,14}$")
            .When(x => x.Phone is not null)
            .WithMessage("Phone number is not valid.");
    }
}