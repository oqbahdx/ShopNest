namespace ShopNest.API.Models.Products;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    string? ShortDescription,
    string Sku,
    decimal Price,
    decimal? CompareAtPrice,
    decimal CostPrice,
    decimal? Weight,
    int StockQuantity,
    int LowStockThreshold,
    bool IsFeatured,
    Guid CategoryId
);