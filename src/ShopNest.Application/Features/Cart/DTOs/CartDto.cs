namespace ShopNest.Application.Features.Cart.DTOs;

public sealed record CartDto(
    Guid Id,
    Guid UserId,
    IReadOnlyList<CartItemDto> Items,
    string? AppliedCouponCode,
    decimal? DiscountAmount,
    decimal SubTotal,
    decimal Total
);

public sealed record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    string? PrimaryImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal,
    bool IsInStock
);