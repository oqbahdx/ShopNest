using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> b)
    {
        b.ToTable("Wishlists");
        b.HasKey(w => w.Id);
        b.HasIndex(w => w.UserId).IsUnique();
        b.HasMany(w => w.Items)
            .WithOne()
            .HasForeignKey(i => i.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> b)
    {
        b.ToTable("WishlistItems");
        b.HasKey(i => i.Id);
        b.HasIndex(i => new { i.WishlistId, i.ProductId }).IsUnique();
        b.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
