using FluentValidation;

namespace ShopNest.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandValidator
    : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}