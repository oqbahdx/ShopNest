using FluentValidation;

namespace ShopNest.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RawRefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
