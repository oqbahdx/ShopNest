using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Products.DTOs;

public sealed record ProductCategoryDto(Guid Id, string Name, string Slug);

public sealed record ProductImageDto(
    Guid Id,
    string ImageUrl,
    string? ThumbnailUrl,
    string? AltText,
    int DisplayOrder,
    bool IsPrimary);

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    string SKU,
    string? Description,
    string? ShortDescription,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? CostPrice,
    decimal? Weight,
    int StockQuantity,
    int LowStockThreshold,
    bool IsFeatured,
    bool IsActive,
    decimal AverageRating,
    int ReviewCount,
    ProductCategoryDto Category,
    IReadOnlyList<ProductImageDto> Images);

public static class ProductMappings
{
    public static ProductDto ToDto(Product product)
        => new(
            product.Id,
            product.Name,
            product.Slug,
            product.SKU,
            product.Description,
            product.ShortDescription,
            product.Price,
            product.CompareAtPrice,
            product.CostPrice,
            product.Weight,
            product.StockQuantity,
            product.LowStockThreshold,
            product.IsFeatured,
            product.IsActive,
            product.AverageRating,
            product.ReviewCount,
            product.Category is null
                ? new ProductCategoryDto(product.CategoryId, string.Empty, string.Empty)
                : new ProductCategoryDto(product.Category.Id, product.Category.Name, product.Category.Slug),
            product.Images
                .OrderBy(i => i.DisplayOrder)
                .ThenBy(i => i.Id)
                .Select(i => new ProductImageDto(
                    i.Id,
                    i.ImageUrl,
                    i.ThumbnailUrl,
                    i.AltText,
                    i.DisplayOrder,
                    i.IsPrimary))
                .ToList());
}
