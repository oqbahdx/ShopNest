using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.UnitTests.Infrastructure;

public static class Fakes
{
    public static Category Category(
        string? name = null,
        string? slug = null,
        Guid? parentCategoryId = null,
        Guid? id = null)
    {
        var category = Domain.Entities.Category.Create(
            name ?? "Electronics",
            slug ?? "electronics",
            null,
            null,
            0,
            parentCategoryId);

        if (id.HasValue)
            category.Id = id.Value;

        return category;
    }

    public static Product Product(
        string? name = null,
        string? sku = null,
        string? slug = null,
        decimal price = 99.99m,
        int stock = 10,
        int threshold = 2,
        bool isFeatured = false,
        Guid? categoryId = null,
        Guid? id = null)
    {
        var product = Domain.Entities.Product.Create(
            name ?? "Test Product",
            null,
            null,
            sku ?? $"SKU-{Guid.NewGuid():N}",
            slug ?? "test-product",
            price,
            null,
            50m,
            null,
            stock,
            threshold,
            isFeatured,
            categoryId ?? Guid.NewGuid());

        if (id.HasValue)
            product.Id = id.Value;

        return product;
    }

    public static ProductImage Image(
        Guid? productId = null,
        string? url = null,
        bool isPrimary = false,
        int displayOrder = 0)
        => ProductImage.Create(
            productId ?? Guid.NewGuid(),
            url ?? "images/product.jpg",
            "Test image",
            displayOrder,
            isPrimary);

    public static Order ActiveOrderFor(Product product)
    {
        var order = new Order
        {
            OrderNumber = "ORD-TEST-0001",
            UserId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            ShippingAddressId = Guid.NewGuid(),
            SubTotal = product.Price,
            TotalAmount = product.Price
        };

        order.Items.Add(OrderItem.FromProduct(order.Id, product, 1));
        return order;
    }
}
