using ShopNest.Application.Features.Categories.Commands.CreateCategory;
using ShopNest.Application.Features.Categories.Commands.DeleteCategory;
using ShopNest.UnitTests.Infrastructure;

namespace ShopNest.UnitTests.Handlers.Commands;

public sealed class CategoryCommandHandlerTests
{
    [Fact]
    public async Task Create_category_generates_slug_and_persists_category()
    {
        await using var db = TestDbContextFactory.Create();
        var handler = new CreateCategoryCommandHandler(db);

        var result = await handler.Handle(
            new CreateCategoryCommand("Smart Phones & Tablets", null, null, 0, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        db.Categories.Single().Slug.Should().Be("smart-phones-tablets");
    }

    [Fact]
    public async Task Create_category_validates_parent_and_depth_limit()
    {
        await using var db = TestDbContextFactory.Create();
        var root = Fakes.Category(name: "Root", slug: "root");
        var level2 = Fakes.Category(name: "Level2", slug: "level2", parentCategoryId: root.Id);
        var level3 = Fakes.Category(name: "Level3", slug: "level3", parentCategoryId: level2.Id);
        db.Categories.AddRange(root, level2, level3);
        await db.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(db);

        var missingParent = await handler.Handle(
            new CreateCategoryCommand("Sub", null, null, 0, Guid.NewGuid()),
            CancellationToken.None);
        var tooDeep = await handler.Handle(
            new CreateCategoryCommand("Level4", null, null, 0, level3.Id),
            CancellationToken.None);

        missingParent.ErrorCode.Should().Be(ErrorCodes.NotFound);
        tooDeep.ErrorCode.Should().Be(ErrorCodes.Conflict);
    }

    [Fact]
    public async Task Create_category_allows_creating_at_max_depth()
    {
        await using var db = TestDbContextFactory.Create();
        var root = Fakes.Category(name: "Root", slug: "root");
        var level2 = Fakes.Category(name: "Level2", slug: "level2", parentCategoryId: root.Id);
        db.Categories.AddRange(root, level2);
        await db.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(db);
        var result = await handler.Handle(
            new CreateCategoryCommand("Level3", null, null, 0, level2.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_category_soft_deletes_empty_leaf_and_blocks_children_or_products()
    {
        await using var db = TestDbContextFactory.Create();
        var empty = Fakes.Category(name: "Empty", slug: "empty");
        var parent = Fakes.Category(name: "Parent", slug: "parent");
        var child = Fakes.Category(name: "Child", slug: "child", parentCategoryId: parent.Id);
        var withProduct = Fakes.Category(name: "With Product", slug: "with-product");
        db.Categories.AddRange(empty, parent, child, withProduct);
        db.Products.Add(Fakes.Product(categoryId: withProduct.Id));
        await db.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(db);

        var deleted = await handler.Handle(new DeleteCategoryCommand(empty.Id), CancellationToken.None);
        var blockedChild = await handler.Handle(new DeleteCategoryCommand(parent.Id), CancellationToken.None);
        var blockedProduct = await handler.Handle(new DeleteCategoryCommand(withProduct.Id), CancellationToken.None);
        var missing = await handler.Handle(new DeleteCategoryCommand(Guid.NewGuid()), CancellationToken.None);

        deleted.IsSuccess.Should().BeTrue();
        blockedChild.ErrorCode.Should().Be(ErrorCodes.Conflict);
        blockedProduct.ErrorCode.Should().Be(ErrorCodes.Conflict);
        missing.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }
}
