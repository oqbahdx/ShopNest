using ShopNest.Application.Features.Products.Commands.CreateProduct;
using ShopNest.Application.Features.Products.Queries.GetProducts;

namespace ShopNest.UnitTests.Validators;

public sealed class ProductValidatorTests
{
    [Fact]
    public async Task Create_product_accepts_valid_command()
    {
        var validator = new CreateProductCommandValidator();
        var result = await validator.ValidateAsync(ValidCreateProduct());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_product_rejects_empty_name(string name)
    {
        var validator = new CreateProductCommandValidator();
        var result = await validator.ValidateAsync(ValidCreateProduct() with { Name = name });
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public async Task Create_product_rejects_invalid_price_stock_and_category()
    {
        var validator = new CreateProductCommandValidator();
        var result = await validator.ValidateAsync(ValidCreateProduct() with
        {
            Price = 0,
            StockQuantity = -1,
            CategoryId = Guid.Empty,
            CompareAtPrice = 0
        });

        result.Errors.Select(e => e.PropertyName).Should().Contain([
            nameof(CreateProductCommand.Price),
            nameof(CreateProductCommand.StockQuantity),
            nameof(CreateProductCommand.CategoryId),
            nameof(CreateProductCommand.CompareAtPrice)
        ]);
    }

    [Fact]
    public async Task Get_products_accepts_valid_sort_combinations()
    {
        var validator = new GetProductsQueryValidator();

        foreach (var query in new[]
        {
            new GetProductsQuery(SortBy: "name", SortOrder: "asc"),
            new GetProductsQuery(SortBy: "price", SortOrder: "desc"),
            new GetProductsQuery(SortBy: "rating", SortOrder: "asc"),
            new GetProductsQuery(SortBy: "createdAt", SortOrder: "desc")
        })
        {
            var result = await validator.ValidateAsync(query);
            result.IsValid.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Get_products_rejects_invalid_paging_sort_and_price_range()
    {
        var validator = new GetProductsQueryValidator();
        var result = await validator.ValidateAsync(new GetProductsQuery(
            Page: 0,
            PageSize: 101,
            SortBy: "color",
            MinPrice: 100,
            MaxPrice: 50));

        result.Errors.Select(e => e.PropertyName).Should().Contain([
            nameof(GetProductsQuery.Page),
            nameof(GetProductsQuery.PageSize),
            nameof(GetProductsQuery.SortBy),
            nameof(GetProductsQuery.MaxPrice)
        ]);
    }

    private static CreateProductCommand ValidCreateProduct() => new(
        "Widget Pro",
        null,
        null,
        "WGT-001",
        49.99m,
        null,
        20m,
        null,
        100,
        5,
        false,
        Guid.NewGuid());
}
