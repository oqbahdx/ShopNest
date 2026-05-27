using FluentValidation;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly HashSet<string> AllowedSortFields =
        new(StringComparer.OrdinalIgnoreCase)
            { "name", "price", "rating", "createdAt" };
    private static readonly HashSet<string> AllowedSortOrders =
        new(StringComparer.OrdinalIgnoreCase)
            { "asc", "desc" };
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("MinPrice cannot be negative.");
        RuleFor(x => x.MaxPrice)
            .GreaterThan(x => x.MinPrice ?? 0)
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
            .WithMessage("MaxPrice must be greater than MinPrice.");
        RuleFor(x => x.MinRating)
            .InclusiveBetween(1, 5)
            .When(x => x.MinRating.HasValue)
            .WithMessage("Rating filter must be between 1 and 5.");
        RuleFor(x => x.SortBy)
            .Must(s => AllowedSortFields.Contains(s))
            .WithMessage("SortBy must be one of: name, price, rating, createdAt.");
        RuleFor(x => x.SortOrder)
            .Must(s => AllowedSortOrders.Contains(s))
            .WithMessage("SortOrder must be 'asc' or 'desc'.");
    }
}