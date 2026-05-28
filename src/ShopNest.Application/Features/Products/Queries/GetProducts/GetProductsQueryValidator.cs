using FluentValidation;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] SortFields = ["name", "price", "rating", "createdAt"];
    private static readonly string[] SortOrders = ["asc", "desc"];

    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).Must(s => SortFields.Contains(s, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.SortOrder).Must(s => SortOrders.Contains(s, StringComparer.OrdinalIgnoreCase));
        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice!.Value)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);
    }
}
