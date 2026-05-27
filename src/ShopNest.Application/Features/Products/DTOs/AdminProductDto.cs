namespace ShopNest.Application.Features.Products.DTOs;

public sealed record AdminProductDto(
    Guid Id,
    string Name,
    string Slug,
    string Sku,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    int StockQuantity,
    int LowStockThreshold,
    bool IsInStock,
    bool IsFeatured,
    bool IsActive,
    bool IsDeleted,
    string CategoryName,
    decimal AverageRating,
    int ReviewCount,
    string? PrimaryImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
