using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;
using ShopNest.Application.Features.Products.Queries.GetProductById;
using ShopNest.Application.Features.Products.Queries.GetProducts;
using ShopNest.UnitTests.Infrastructure;

namespace ShopNest.UnitTests.Handlers.Queries;

public sealed class ProductQueryHandlerTests
{
    [Fact]
    public async Task Get_products_filters_search_price_and_paginates()
    {
        await using var db = TestDbContextFactory.Create();
        var category = Fakes.Category();
        db.Categories.Add(category);
        db.Products.AddRange(
            Fakes.Product(name: "Wireless Mouse", sku: "WM-001", price: 50m, categoryId: category.Id),
            Fakes.Product(name: "Mechanical Keyboard", sku: "KB-001", price: 200m, categoryId: category.Id),
            Fakes.Product(name: "Mouse Pad", sku: "MP-001", price: 10m, categoryId: category.Id));
        await db.SaveChangesAsync();

        var handler = new GetProductsQueryHandler(db);

        var search = await handler.Handle(new GetProductsQuery(Search: "mouse", SortBy: "name", SortOrder: "asc"), CancellationToken.None);
        var price = await handler.Handle(new GetProductsQuery(MinPrice: 20m, MaxPrice: 100m), CancellationToken.None);
        var page = await handler.Handle(new GetProductsQuery(Page: 1, PageSize: 2), CancellationToken.None);

        search.Value!.Items.Should().HaveCount(2);
        price.Value!.Items.Should().ContainSingle(p => p.Price == 50m);
        page.Value!.Items.Should().HaveCount(2);
        page.Value.TotalCount.Should().Be(3);
        page.Value.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Get_product_by_id_returns_product_with_category_and_images()
    {
        await using var db = TestDbContextFactory.Create();
        var category = Fakes.Category();
        var product = Fakes.Product(name: "Laptop Pro", sku: "LP-001", categoryId: category.Id);
        var image = Fakes.Image(product.Id, isPrimary: true);
        db.Categories.Add(category);
        db.Products.Add(product);
        db.ProductImages.Add(image);
        await db.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(db);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(product.Id);
        result.Value.Category.Id.Should().Be(category.Id);
        result.Value.Images.Should().ContainSingle(i => i.IsPrimary);
    }

    [Fact]
    public async Task Get_product_by_id_returns_not_found_for_missing_or_soft_deleted_product()
    {
        await using var db = TestDbContextFactory.Create();
        var product = Fakes.Product();
        db.Products.Add(product);
        await db.SaveChangesAsync();
        db.Products.Remove(product);
        await db.SaveChangesAsync();

        var handler = new GetProductByIdQueryHandler(db);
        var missing = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);
        var deleted = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        missing.ErrorCode.Should().Be(ErrorCodes.NotFound);
        deleted.ErrorCode.Should().Be(ErrorCodes.NotFound);
        (await db.Products.IgnoreQueryFilters().SingleAsync()).IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Get_featured_products_filters_limits_and_uses_cache()
    {
        await using var db = TestDbContextFactory.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var category = Fakes.Category();
        db.Categories.Add(category);
        db.Products.AddRange(
            Fakes.Product(sku: "F1", isFeatured: true, categoryId: category.Id),
            Fakes.Product(sku: "N1", isFeatured: false, categoryId: category.Id));
        await db.SaveChangesAsync();

        var handler = new GetFeaturedProductsQueryHandler(db, cache);
        var first = await handler.Handle(new GetFeaturedProductsQuery(Top: 10), CancellationToken.None);

        db.Products.Add(Fakes.Product(sku: "F2", isFeatured: true, categoryId: category.Id));
        await db.SaveChangesAsync();

        var second = await handler.Handle(new GetFeaturedProductsQuery(Top: 10), CancellationToken.None);

        first.Value!.Should().ContainSingle(p => p.IsFeatured);
        second.Value!.Should().HaveCount(1);
    }
}
