namespace ShopNest.API.Models.Products;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    string? ShortDescription,
    string Sku,
    decimal Price,
    decimal? CompareAtPrice,
    decimal CostPrice,
    decimal? Weight,
    int LowStockThreshold,
    bool IsFeatured,
    bool IsActive,
    Guid CategoryId
);