namespace ShopNest.Application.Features.Products.DTOs;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    int ProductCount,
    IReadOnlyList<CategoryDto> SubCategories
);