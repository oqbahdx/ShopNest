using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> b)
    {
        b.HasKey(i => i.Id);
        b.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        b.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");

        // A product can only appear once per cart
        b.HasIndex(i => new { i.CartId, i.ProductId }).IsUnique();

        b.HasOne(i => i.Product)
         .WithMany(p => p.CartItems)
         .HasForeignKey(i => i.ProductId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
