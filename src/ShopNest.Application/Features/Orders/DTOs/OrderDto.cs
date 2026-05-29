using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.DTOs;

public sealed record OrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    string ShippingFullName,
    string ShippingLine1,
    string? ShippingLine2,
    string ShippingCity,
    string ShippingState,
    string ShippingPostalCode,
    string ShippingCountry,
    string? CouponCode,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal ShippingCost,
    decimal TaxAmount,
    decimal TotalAmount,
    string? TrackingNumber,
    IReadOnlyList<OrderItemDto> Items,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    string? ProductImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
);