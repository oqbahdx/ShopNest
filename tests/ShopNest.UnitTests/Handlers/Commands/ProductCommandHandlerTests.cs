using Microsoft.EntityFrameworkCore;
using Moq;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Features.Products.Commands.CreateProduct;
using ShopNest.Application.Features.Products.Commands.DeleteProduct;
using ShopNest.Application.Features.Products.Commands.DeleteProductImage;
using ShopNest.Application.Features.Products.Commands.UpdateProduct;
using ShopNest.Application.Features.Products.Commands.UpdateStock;
using ShopNest.UnitTests.Infrastructure;

namespace ShopNest.UnitTests.Handlers.Commands;

public sealed class ProductCommandHandlerTests
{
    [Fact]
    public async Task Create_product_persists_product_with_unique_slug()
    {
        await using var db = TestDbContextFactory.Create();
        var category = Fakes.Category();
        db.Categories.Add(category);
        db.Products.Add(Fakes.Product(name: "Wireless Mouse", slug: "wireless-mouse", categoryId: category.Id));
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db);
        var result = await handler.Handle(ValidCreate(category.Id, "Wireless Mouse", "MOUSE-02"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        db.Products.Single(p => p.SKU == "MOUSE-02").Slug.Should().Be("wireless-mouse-1");
    }

    [Fact]
    public async Task Create_product_returns_not_found_for_missing_category_and_conflict_for_duplicate_sku()
    {
        await using var db = TestDbContextFactory.Create();
        var category = Fakes.Category();
        db.Categories.Add(category);
        db.Products.Add(Fakes.Product(sku: "DUPE-SKU", categoryId: category.Id));
        await db.SaveChangesAsync();

        var handler = new CreateProductCommandHandler(db);

        var missing = await handler.Handle(ValidCreate(Guid.NewGuid(), sku: "NEW-SKU"), CancellationToken.None);
        var duplicate = await handler.Handle(ValidCreate(category.Id, sku: "DUPE-SKU"), CancellationToken.None);

        missing.ErrorCode.Should().Be(ErrorCodes.NotFound);
        duplicate.ErrorCode.Should().Be(ErrorCodes.Conflict);
    }

    [Fact]
    public async Task Update_product_updates_values_and_regenerates_slug_when_name_changes()
    {
        await using var db = TestDbContextFactory.Create();
        var category = Fakes.Category();
        var product = Fakes.Product(name: "Original", sku: "ORIG-001", slug: "original", categoryId: category.Id);
        db.Categories.Add(category);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(db);
        var result = await handler.Handle(ValidUpdate(product.Id, category.Id, "Brand New Name", product.SKU), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var updated = db.Products.Single();
        updated.Name.Should().Be("Brand New Name");
        updated.Slug.Should().Be("brand-new-name");
    }

    [Fact]
    public async Task Update_product_preserves_slug_when_name_is_unchanged_and_rejects_duplicate_sku()
    {
        await using var db = TestDbContextFactory.Create();
        var category = Fakes.Category();
        var p1 = Fakes.Product(name: "Original", sku: "SKU-A", slug: "original", categoryId: category.Id);
        var p2 = Fakes.Product(name: "Other", sku: "SKU-B", slug: "other", categoryId: category.Id);
        db.Categories.Add(category);
        db.Products.AddRange(p1, p2);
        await db.SaveChangesAsync();

        var handler = new UpdateProductCommandHandler(db);
        var unchanged = await handler.Handle(ValidUpdate(p1.Id, category.Id, p1.Name, p1.SKU), CancellationToken.None);
        var duplicate = await handler.Handle(ValidUpdate(p1.Id, category.Id, p1.Name, p2.SKU), CancellationToken.None);

        unchanged.IsSuccess.Should().BeTrue();
        db.Products.Single(p => p.Id == p1.Id).Slug.Should().Be("original");
        duplicate.ErrorCode.Should().Be(ErrorCodes.Conflict);
    }

    [Fact]
    public async Task Delete_product_soft_deletes_and_blocks_active_orders()
    {
        await using var db = TestDbContextFactory.Create();
        var product = Fakes.Product();
        var blocked = Fakes.Product(sku: "BLOCKED");
        var activeOrder = Fakes.ActiveOrderFor(blocked);
        db.Products.AddRange(product, blocked);
        db.Orders.Add(activeOrder);
        await db.SaveChangesAsync();

        var handler = new DeleteProductCommandHandler(db);
        var deletedResult = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);
        var blockedResult = await handler.Handle(new DeleteProductCommand(blocked.Id), CancellationToken.None);

        deletedResult.IsSuccess.Should().BeTrue();
        blockedResult.ErrorCode.Should().Be(ErrorCodes.Conflict);
        var deleted = await db.Products.IgnoreQueryFilters().SingleAsync(p => p.Id == product.Id);
        deleted.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Update_stock_sets_quantity_and_returns_not_found_for_missing_product()
    {
        await using var db = TestDbContextFactory.Create();
        var product = Fakes.Product(stock: 50);
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var handler = new UpdateStockCommandHandler(db);
        var success = await handler.Handle(new UpdateStockCommand(product.Id, 75, "Manual"), CancellationToken.None);
        var missing = await handler.Handle(new UpdateStockCommand(Guid.NewGuid(), 10, "Manual"), CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        db.Products.Single().StockQuantity.Should().Be(75);
        missing.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }

    [Fact]
    public async Task Delete_product_image_removes_images_and_promotes_replacement_primary()
    {
        await using var db = TestDbContextFactory.Create();
        var product = Fakes.Product();
        var primary = Fakes.Image(product.Id, isPrimary: true, displayOrder: 0);
        var secondary = Fakes.Image(product.Id, isPrimary: false, displayOrder: 1);
        db.Products.Add(product);
        db.ProductImages.AddRange(primary, secondary);
        await db.SaveChangesAsync();

        var files = new Mock<IFileService>();
        files.Setup(f => f.DeleteAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new DeleteProductImageCommandHandler(db, files.Object);

        var result = await handler.Handle(new DeleteProductImageCommand(product.Id, primary.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        db.ProductImages.Single().Id.Should().Be(secondary.Id);
        db.ProductImages.Single().IsPrimary.Should().BeTrue();
        files.Verify(f => f.DeleteAsync(primary.ImageUrl, "products", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_product_image_rejects_only_image_and_wrong_product_image()
    {
        await using var db = TestDbContextFactory.Create();
        var product = Fakes.Product();
        var image = Fakes.Image(product.Id, isPrimary: true);
        db.Products.Add(product);
        db.ProductImages.Add(image);
        await db.SaveChangesAsync();

        var files = Mock.Of<IFileService>();
        var handler = new DeleteProductImageCommandHandler(db, files);

        var onlyImage = await handler.Handle(new DeleteProductImageCommand(product.Id, image.Id), CancellationToken.None);
        var missing = await handler.Handle(new DeleteProductImageCommand(product.Id, Guid.NewGuid()), CancellationToken.None);

        onlyImage.ErrorCode.Should().Be(ErrorCodes.Conflict);
        missing.ErrorCode.Should().Be(ErrorCodes.NotFound);
    }

    private static CreateProductCommand ValidCreate(Guid categoryId, string name = "Test Product", string sku = "SKU-TEST")
        => new(name, null, null, sku, 29.99m, null, 10m, null, 50, 5, false, categoryId);

    private static UpdateProductCommand ValidUpdate(Guid productId, Guid categoryId, string name, string sku)
        => new(productId, name, null, null, sku, 49.99m, null, 20m, null, 5, false, true, categoryId);
}
