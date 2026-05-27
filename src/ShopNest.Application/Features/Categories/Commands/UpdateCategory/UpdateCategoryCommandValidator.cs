using FluentValidation;

namespace ShopNest.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandValidator
    : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(200).WithMessage("Category name must not exceed 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description must not exceed 1000 characters.");
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
        // Cannot be own parent — deeper circular check is in the handler
        RuleFor(x => x.ParentCategoryId)
            .Must((cmd, parentId) => parentId != cmd.Id)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("A category cannot be its own parent.");
    }
}