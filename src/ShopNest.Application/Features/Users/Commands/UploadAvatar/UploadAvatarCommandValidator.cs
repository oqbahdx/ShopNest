using FluentValidation;

namespace ShopNest.Application.Features.Users.Commands.UploadAvatar;

public sealed class UploadAvatarCommandValidator
    : AbstractValidator<UploadAvatarCommand>
{
    private static readonly HashSet<string> AllowedTypes =
        new(StringComparer.OrdinalIgnoreCase)
            { "image/jpeg", "image/png", "image/webp" };

    private const long MaxBytes = 5L * 1024 * 1024; // 5 MB

    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .Must(f => f.Length <= MaxBytes)
            .WithMessage("Avatar must not exceed 5 MB.")
            .Must(f => AllowedTypes.Contains(f.ContentType))
            .WithMessage("Only JPEG, PNG, and WebP images are allowed.");
    }
}