using FluentValidation;

namespace ShopNest.Application.Features.Products.Queries.SearchProducts;

public sealed class SearchProductsQueryValidator
    : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.Search)
            .NotEmpty().WithMessage("Search term is required.")
            .MinimumLength(2).WithMessage("Search term must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.");
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}