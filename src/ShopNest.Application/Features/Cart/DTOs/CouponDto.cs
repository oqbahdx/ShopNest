namespace ShopNest.Application.Features.Cart.DTOs;

public sealed record CouponDto(
    Guid Id,
    string Code,
    string DiscountType, // Percentage | FixedAmount
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? UsageLimit,
    int UsageCount,
    DateTime? ExpiresAt,
    bool IsActive
);

public sealed record CouponValidationDto(
    bool IsValid,
    string? ErrorMessage,
    decimal DiscountAmount,
    string DiscountType
);