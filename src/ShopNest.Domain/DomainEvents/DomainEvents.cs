using MediatR;

namespace ShopNest.Domain.DomainEvents;

// ── Order lifecycle events ──────────────────────────────────────────────────
// Published by Order entity methods or Phase 4 webhook handler.
// Consumed by Phase 5 INotificationHandler implementations.

public sealed record OrderPlacedDomainEvent(
    Guid OrderId,
    Guid UserId,
    string OrderNumber,
    decimal TotalAmount
) : INotification;

public sealed record OrderConfirmedDomainEvent(
    Guid OrderId,
    Guid UserId
) : INotification;

public sealed record OrderShippedDomainEvent(
    Guid OrderId,
    Guid UserId,
    string OrderNumber,
    string TrackingNumber
) : INotification;

public sealed record OrderDeliveredDomainEvent(
    Guid OrderId,
    Guid UserId,
    string OrderNumber
) : INotification;

public sealed record OrderCancelledDomainEvent(
    Guid OrderId,
    Guid UserId
) : INotification;

// ── Product lifecycle events ────────────────────────────────────────────────

public sealed record LowStockDomainEvent(
    Guid ProductId,
    string ProductName,
    int CurrentStock,
    int Threshold
) : INotification;
