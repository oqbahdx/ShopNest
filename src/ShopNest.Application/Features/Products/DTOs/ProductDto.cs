namespace ShopNest.Application.Features.Products.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ShortDescription,
    string Sku,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? Weight,
    bool IsFeatured,
    bool IsActive,
    decimal AverageRating,
    int ReviewCount,
    bool IsInStock,
    ProductCategoryDto Category,
    IReadOnlyList<ProductImageDto> Images
);

public sealed record ProductImageDto(
    Guid Id,
    string Url,
    string? AltText,
    int DisplayOrder,
    bool IsPrimary
);

public sealed record ProductCategoryDto(
    Guid Id,
    string Name,
    string Slug
);
