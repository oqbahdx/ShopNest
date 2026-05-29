using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.DTOs;

public sealed record AdminOrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    Guid UserId,
    string CustomerEmail,
    int ItemCount,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    string? CouponCode,
    string? TrackingNumber,
    DateTime CreatedAt
);

public sealed record OrderSummaryDto(
    int Pending,
    int Confirmed,
    int Processing,
    int Shipped,
    int Delivered,
    int Cancelled,
    int ReturnRequested
);