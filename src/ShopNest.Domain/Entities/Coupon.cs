using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Coupon : AuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DiscountType DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int UsedCount { get; set; }

    public int UsageCount
    {
        get => UsedCount;
        set => UsedCount = value;
    }

    public bool IsOnePerUser { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public static Coupon Create(
        string code,
        DiscountType discountType,
        decimal discountValue,
        decimal? minimumOrderAmount,
        decimal? maximumDiscountAmount,
        int? usageLimit,
        DateTime? expiresAt) => new()
    {
        Id                   = Guid.NewGuid(),
        Code                 = code,
        DiscountType         = discountType,
        DiscountValue        = discountValue,
        MinimumOrderAmount   = minimumOrderAmount,
        MaximumDiscountAmount = maximumDiscountAmount,
        UsageLimit           = usageLimit,
        ExpiresAt            = expiresAt,
        IsActive             = true
    };

    public void Update(
        DiscountType discountType,
        decimal discountValue,
        decimal? minimumOrderAmount,
        decimal? maximumDiscountAmount,
        int? usageLimit,
        DateTime? expiresAt)
    {
        DiscountType          = discountType;
        DiscountValue         = discountValue;
        MinimumOrderAmount    = minimumOrderAmount;
        MaximumDiscountAmount = maximumDiscountAmount;
        UsageLimit            = usageLimit;
        ExpiresAt             = expiresAt;
    }

    public void Deactivate() => IsActive = false;

    public bool IsValid(decimal orderAmount) =>
        IsActive
        && (!ExpiresAt.HasValue || ExpiresAt > DateTime.UtcNow)
        && (!UsageLimit.HasValue || UsedCount < UsageLimit)
        && (!MinimumOrderAmount.HasValue || orderAmount >= MinimumOrderAmount.Value);

    public decimal CalculateDiscount(decimal subTotal) =>
        CalculateDiscount(subTotal, shippingCost: 0m);

    public decimal CalculateDiscount(decimal subTotal, decimal shippingCost)
    {
        var amount = DiscountType switch
        {
            DiscountType.Percentage   => Math.Round(subTotal * DiscountValue / 100m, 2),
            DiscountType.FixedAmount  => DiscountValue,
            DiscountType.FreeShipping => shippingCost,
            _                         => 0m
        };

        if (DiscountType == DiscountType.Percentage && MaximumDiscountAmount.HasValue)
            amount = Math.Min(amount, MaximumDiscountAmount.Value);

        return Math.Min(amount, subTotal);
    }

    public void IncrementUsage() => UsedCount++;
}
