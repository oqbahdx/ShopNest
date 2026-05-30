using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Exceptions;

namespace ShopNest.Domain.Entities;

public sealed class Wishlist : AuditableEntity
{
    public Guid UserId { get; private set; }

    public ICollection<WishlistItem> Items { get; private set; } = new List<WishlistItem>();

    private Wishlist() { }

    public static Wishlist Create(Guid userId) => new()
    {
        Id     = Guid.NewGuid(),
        UserId = userId
    };

    public void AddItem(Guid productId)
    {
        if (Items.Any(i => i.ProductId == productId))
            throw new DomainException("Product is already in the wishlist.");

        if (Items.Count >= 100)
            throw new DomainException(
                "Wishlist cannot contain more than 100 items.");

        Items.Add(WishlistItem.Create(Id, productId));
    }

    public void RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId)
                   ?? throw new DomainException("Item not found in wishlist.");

        Items.Remove(item);
    }
}

public sealed class WishlistItem : BaseEntity
{
    public Guid     WishlistId { get; private set; }
    public Guid     ProductId  { get; private set; }
    public DateTime AddedAt    { get; private set; }

    // Navigation
    public Product Product { get; private set; } = null!;

    private WishlistItem() { }

    public static WishlistItem Create(Guid wishlistId, Guid productId) => new()
    {
        Id         = Guid.NewGuid(),
        WishlistId = wishlistId,
        ProductId  = productId,
        AddedAt    = DateTime.UtcNow
    };
}