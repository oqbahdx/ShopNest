using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.DTOs;

public sealed record OrderListDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    int ItemCount,
    decimal TotalAmount,
    string? TrackingNumber,
    DateTime CreatedAt
);