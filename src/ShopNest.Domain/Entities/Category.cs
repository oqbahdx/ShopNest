using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class Category : AuditableEntity, ISoftDeletable
{
    public string  Name            { get; set; } = string.Empty;
    public string  Slug            { get; set; } = string.Empty;
    public string? Description     { get; set; }
    public string? ImageUrl        { get; set; }
    public int     DisplayOrder    { get; set; } = 0;
    public bool    IsActive        { get; set; } = true;
    public Guid?   ParentCategoryId { get; set; }

    // ISoftDeletable
    public bool      IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid?     DeletedBy { get; set; }

    // Navigation
    public Category?              ParentCategory  { get; set; }
    public ICollection<Category>  SubCategories   { get; set; } = [];
    public ICollection<Product>   Products        { get; set; } = [];

    public static Category Create(
        string name,
        string slug,
        string? description,
        string? imageUrl,
        int displayOrder,
        Guid? parentCategoryId)
    {
        return new Category
        {
            Name = name,
            Slug = slug,
            Description = description,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder,
            ParentCategoryId = parentCategoryId
        };
    }

    public void Update(
        string name,
        string slug,
        string? description,
        string? imageUrl,
        int displayOrder,
        Guid? parentCategoryId)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        ParentCategoryId = parentCategoryId;
    }
}
