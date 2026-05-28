using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Coupon : AuditableEntity
{
    public string       Code                 { get; set; } = string.Empty;
    public string?      Description          { get; set; }
    public DiscountType DiscountType         { get; set; }
    public decimal      DiscountValue        { get; set; }   // % or fixed amount
    public decimal      MinimumOrderAmount   { get; set; } = 0;
    public decimal?     MaximumDiscountAmount { get; set; }  // cap for percentage discounts
    public int?         UsageLimit           { get; set; }   // null = unlimited
    public int          UsedCount            { get; set; } = 0;
    public bool         IsOnePerUser         { get; set; } = false;
    public bool         IsActive             { get; set; } = true;
    public DateTime?    ExpiresAt            { get; set; }

    // Navigation
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Cart>  Carts  { get; set; } = [];

    // Domain behaviour
    public bool IsValid(decimal orderAmount) =>
        IsActive
        && (ExpiresAt is null || ExpiresAt > DateTime.UtcNow)
        && (UsageLimit is null || UsedCount < UsageLimit)
        && orderAmount >= MinimumOrderAmount;

    public decimal CalculateDiscount(decimal subTotal, decimal shippingCost)
    {
        var discount = DiscountType switch
        {
            DiscountType.Percentage   => Math.Round(subTotal * DiscountValue / 100, 2),
            DiscountType.FixedAmount  => DiscountValue,
            DiscountType.FreeShipping => shippingCost,
            _ => 0m
        };

        // apply cap for percentage discounts
        if (DiscountType == DiscountType.Percentage && MaximumDiscountAmount.HasValue)
            discount = Math.Min(discount, MaximumDiscountAmount.Value);

        return Math.Min(discount, subTotal); // never exceed order value
    }

    public void IncrementUsage() => UsedCount++;
}
