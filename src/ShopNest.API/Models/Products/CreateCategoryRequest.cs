namespace ShopNest.API.Models.Products;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    Guid? ParentCategoryId
);