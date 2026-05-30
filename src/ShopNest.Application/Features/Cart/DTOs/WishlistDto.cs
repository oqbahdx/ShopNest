namespace ShopNest.Application.Features.Cart.DTOs;

public sealed record WishlistDto(
    Guid Id,
    IReadOnlyList<WishlistItemDto> Items
);

public sealed record WishlistItemDto(
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    string? PrimaryImageUrl,
    decimal Price,
    bool IsInStock,
    DateTime AddedAt
);