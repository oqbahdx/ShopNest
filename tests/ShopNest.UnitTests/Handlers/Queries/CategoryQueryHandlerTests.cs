using Microsoft.Extensions.Caching.Memory;
using ShopNest.Application.Features.Categories.Queries.GetCategories;
using ShopNest.UnitTests.Infrastructure;

namespace ShopNest.UnitTests.Handlers.Queries;

public sealed class CategoryQueryHandlerTests
{
    [Fact]
    public async Task Get_categories_returns_tree_and_recursive_product_counts()
    {
        await using var db = TestDbContextFactory.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var root = Fakes.Category(name: "Root", slug: "root");
        var child = Fakes.Category(name: "Child", slug: "child", parentCategoryId: root.Id);
        db.Categories.AddRange(root, child);
        db.Products.Add(Fakes.Product(categoryId: child.Id));
        await db.SaveChangesAsync();

        var handler = new GetCategoriesQueryHandler(db, cache);
        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().ContainSingle();
        result.Value[0].SubCategories.Should().ContainSingle();
        result.Value[0].ProductCount.Should().Be(1);
        result.Value[0].SubCategories[0].ProductCount.Should().Be(1);
    }

    [Fact]
    public async Task Get_categories_returns_empty_list_and_uses_cache()
    {
        await using var db = TestDbContextFactory.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetCategoriesQueryHandler(db, cache);

        var empty = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);
        empty.Value!.Should().BeEmpty();

        db.Categories.Add(Fakes.Category(name: "Books", slug: "books"));
        await db.SaveChangesAsync();

        var cached = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);
        cached.Value!.Should().BeEmpty();
    }
}
