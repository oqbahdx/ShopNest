using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

/// <summary>
/// Snapshot of a product at the time the order was placed.
/// Price/name changes after placement do NOT affect existing order items.
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid    OrderId          { get; set; }
    public Guid    ProductId        { get; set; }

    // ── Snapshots (intentionally denormalised) ────────────────────────────────
    public string  ProductName      { get; set; } = string.Empty;
    public string? ProductImageUrl  { get; set; }
    public string  ProductSKU       { get; set; } = string.Empty;

    public decimal UnitPrice        { get; set; }
    public int     Quantity         { get; set; }
    public decimal TotalPrice       { get; set; }

    // Navigation
    public Order   Order   { get; set; } = null!;
    public Product Product { get; set; } = null!;

    public static OrderItem FromProduct(Guid orderId, Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        return new OrderItem
        {
            OrderId         = orderId,
            ProductId       = product.Id,
            ProductName     = product.Name,
            ProductImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            ProductSKU      = product.SKU,
            UnitPrice       = product.Price,
            Quantity        = quantity,
            TotalPrice      = Math.Round(product.Price * quantity, 2)
        };
    }
}
