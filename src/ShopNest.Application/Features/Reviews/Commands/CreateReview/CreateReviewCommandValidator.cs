using FluentValidation;

namespace ShopNest.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewCommandValidator
    : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Review title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Review comment is required.")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");
    }
}