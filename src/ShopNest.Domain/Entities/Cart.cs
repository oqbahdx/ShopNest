using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class Cart : AuditableEntity
{
    public Guid    UserId         { get; set; }
    public Guid?   CouponId       { get; set; }
    public decimal SubTotal       { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal ShippingCost   { get; set; } = 0;
    public decimal Total          { get; set; } = 0;

    // Navigation
   
    public Coupon?            Coupon { get; set; }
    public ICollection<CartItem> Items { get; set; } = [];

    // Domain behaviour
    public void Recalculate()
    {
        SubTotal = Items.Sum(i => i.TotalPrice);

        DiscountAmount = Coupon is null ? 0 : Coupon.DiscountType switch
        {
            Enums.DiscountType.Percentage   => Math.Round(SubTotal * Coupon.DiscountValue / 100, 2),
            Enums.DiscountType.FixedAmount  => Math.Min(Coupon.DiscountValue, SubTotal),
            Enums.DiscountType.FreeShipping => ShippingCost,
            _ => 0
        };

        Total = SubTotal - DiscountAmount + ShippingCost;
    }

    public void AddItem(CartItem item)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing is not null)
            existing.UpdateQuantity(existing.Quantity + item.Quantity);
        else
            Items.Add(item);

        Recalculate();
    }

    public void RemoveItem(Guid cartItemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == cartItemId)
                   ?? throw new InvalidOperationException("Cart item not found.");
        Items.Remove(item);
        Recalculate();
    }

    public void Clear()
    {
        Items.Clear();
        CouponId       = null;
        Coupon         = null;
        DiscountAmount = 0;
        SubTotal       = 0;
        Total          = 0;
    }
}
