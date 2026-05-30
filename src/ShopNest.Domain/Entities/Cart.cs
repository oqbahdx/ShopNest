using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Cart : AuditableEntity
{
    public Guid UserId { get; set; }

    public Guid? CouponId { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal Total { get; set; }

    public Coupon? Coupon { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public static Cart Create(Guid userId) => new()
    {
        Id     = Guid.NewGuid(),
        UserId = userId
    };

    public void Recalculate()
    {
        SubTotal = Items.Sum(i => i.TotalPrice);

        DiscountAmount = Coupon is null
            ? 0m
            : Coupon.CalculateDiscount(SubTotal, ShippingCost);

        Total = SubTotal - DiscountAmount + ShippingCost;
    }

    public void AddItem(Guid productId, decimal unitPrice, int quantity)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + quantity);
        }
        else
        {
            Items.Add(new CartItem
            {
                Id         = Guid.NewGuid(),
                CartId     = Id,
                ProductId  = productId,
                Quantity   = quantity,
                UnitPrice  = unitPrice,
                TotalPrice = Math.Round(unitPrice * quantity, 2)
            });
        }

        Recalculate();
    }

    public void UpdateItemQuantity(Guid cartItemId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.Id == cartItemId)
                   ?? throw new InvalidOperationException("Cart item not found.");

        item.UpdateQuantity(quantity);
        Recalculate();
    }

    public void RemoveItem(Guid cartItemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == cartItemId)
                   ?? throw new InvalidOperationException("Cart item not found.");

        Items.Remove(item);
        Recalculate();
    }

    public void ApplyCoupon(Coupon coupon)
    {
        CouponId = coupon.Id;
        Coupon   = coupon;
    }

    public void RemoveCoupon()
    {
        CouponId = null;
        Coupon   = null;
    }

    public void Clear()
    {
        Items.Clear();
        CouponId       = null;
        Coupon         = null;
        DiscountAmount = 0m;
        SubTotal       = 0m;
        Total          = 0m;
    }
}
