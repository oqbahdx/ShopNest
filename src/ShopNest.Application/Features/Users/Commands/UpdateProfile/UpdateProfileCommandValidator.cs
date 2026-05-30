using FluentValidation;

namespace ShopNest.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileCommandValidator
    : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .When(x => x.FirstName is not null)
            .WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => x.LastName is not null)
            .WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .Matches(@"^+?[1-9]d{6,14}$")
            .When(x => x.Phone is not null)
            .WithMessage("Phone number is not valid.");
    }
}