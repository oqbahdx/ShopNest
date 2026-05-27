namespace ShopNest.Application.Features.Products.DTOs;

public sealed record ProductListDto(
    Guid Id,
    string Name,
    string Slug,
    decimal Price,
    decimal? CompareAtPrice,
    string? PrimaryImageUrl,
    string CategoryName,
    decimal AverageRating,
    int ReviewCount,
    bool IsInStock,
    bool IsFeatured
);
