namespace ShopNest.API.Models.Products;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    Guid? ParentCategoryId
);