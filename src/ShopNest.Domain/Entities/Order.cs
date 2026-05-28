using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Order : AuditableEntity, ISoftDeletable
{
    public string      OrderNumber       { get; set; } = string.Empty;
    public Guid        UserId            { get; set; }
    public OrderStatus Status            { get; set; } = OrderStatus.Pending;
    public decimal     SubTotal          { get; set; }
    public decimal     DiscountAmount    { get; set; } = 0;
    public decimal     ShippingCost      { get; set; } = 0;
    public decimal     TaxAmount         { get; set; } = 0;
    public decimal     TotalAmount       { get; set; }
    public Guid        ShippingAddressId { get; set; }
    public Guid?       CouponId          { get; set; }
    public string?     TrackingNumber    { get; set; }
    public string?     Notes             { get; set; }
    public string?     CancelReason      { get; set; }
    public DateTime?   CancelledAt       { get; set; }
    public DateTime?   ShippedAt         { get; set; }
    public DateTime?   DeliveredAt       { get; set; }

    // ISoftDeletable
    public bool      IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid?     DeletedBy { get; set; }

    // Navigation
    public Address              ShippingAddress { get; set; } = null!;
    public Coupon?              Coupon          { get; set; }
    public Payment?             Payment         { get; set; }
    public ICollection<OrderItem> Items         { get; set; } = [];

    // ── Domain behaviour ──────────────────────────────────────────────────────
    private static readonly IReadOnlyDictionary<OrderStatus, IReadOnlySet<OrderStatus>> _allowedTransitions =
        new Dictionary<OrderStatus, IReadOnlySet<OrderStatus>>
        {
            [OrderStatus.Pending]         = new HashSet<OrderStatus> { OrderStatus.Confirmed,  OrderStatus.Cancelled },
            [OrderStatus.Confirmed]       = new HashSet<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled },
            [OrderStatus.Processing]      = new HashSet<OrderStatus> { OrderStatus.Shipped },
            [OrderStatus.Shipped]         = new HashSet<OrderStatus> { OrderStatus.Delivered,  OrderStatus.ReturnRequested },
            [OrderStatus.Delivered]       = new HashSet<OrderStatus> { OrderStatus.ReturnRequested },
            [OrderStatus.Cancelled]       = new HashSet<OrderStatus>(),
            [OrderStatus.ReturnRequested] = new HashSet<OrderStatus> { OrderStatus.Returned,   OrderStatus.Confirmed },
            [OrderStatus.Returned]        = new HashSet<OrderStatus>(),
        };

    public void TransitionTo(OrderStatus newStatus, string? reason = null)
    {
        if (!_allowedTransitions[Status].Contains(newStatus))
            throw new InvalidOperationException(
                $"Order cannot transition from '{Status}' to '{newStatus}'.");

        Status = newStatus;

        switch (newStatus)
        {
            case OrderStatus.Cancelled:
                CancelReason  = reason;
                CancelledAt   = DateTime.UtcNow;
                break;
            case OrderStatus.Shipped:
                ShippedAt     = DateTime.UtcNow;
                break;
            case OrderStatus.Delivered:
                DeliveredAt   = DateTime.UtcNow;
                break;
        }
    }

    public bool CanCancel()  => Status is OrderStatus.Pending or OrderStatus.Confirmed;
    public bool CanReturn()  => Status == OrderStatus.Delivered
                                && DeliveredAt.HasValue
                                && DateTime.UtcNow <= DeliveredAt.Value.AddDays(30);

    public static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(10000, 99999)}";
}
