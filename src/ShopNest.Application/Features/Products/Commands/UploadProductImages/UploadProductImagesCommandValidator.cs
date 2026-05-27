using FluentValidation;

namespace ShopNest.Application.Features.Products.Commands.UploadProductImages;

public sealed class UploadProductImagesCommandValidator
    : AbstractValidator<UploadProductImagesCommand>
{
    private static readonly HashSet<string> AllowedTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };

    private const long MaxFileSizeBytes = 10L * 1024 * 1024; // 10 MB

    public UploadProductImagesCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.Images)
            .NotEmpty().WithMessage("At least one image is required.")
            .Must(imgs => imgs.Count <= 10)
            .WithMessage("A maximum of 10 images can be uploaded at once.");
        RuleForEach(x => x.Images)
            .Must(f => f.Length <= MaxFileSizeBytes)
            .WithMessage("Each image must not exceed 10 MB.")
            .Must(f => AllowedTypes.Contains(f.ContentType))
            .WithMessage("Only JPEG, PNG, and WebP images are allowed.");
    }
}