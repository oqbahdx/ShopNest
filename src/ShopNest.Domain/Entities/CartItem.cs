using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid    CartId     { get; set; }
    public Guid    ProductId  { get; set; }
    public int     Quantity   { get; set; }
    public decimal UnitPrice  { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation
    public Cart    Cart    { get; set; } = null!;
    public Product Product { get; set; } = null!;

    // Domain behaviour
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be at least 1.", nameof(newQuantity));
        Quantity   = newQuantity;
        TotalPrice = Math.Round(UnitPrice * Quantity, 2);
    }

    public static CartItem Create(Guid cartId, Product product, int quantity)
    {
        if (product.StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock for '{product.Name}'.");

        return new CartItem
        {
            CartId     = cartId,
            ProductId  = product.Id,
            Quantity   = quantity,
            UnitPrice  = product.Price,
            TotalPrice = Math.Round(product.Price * quantity, 2)
        };
    }
}
