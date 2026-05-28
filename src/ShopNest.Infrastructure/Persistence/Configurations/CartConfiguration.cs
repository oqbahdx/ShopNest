using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.SubTotal).HasColumnType("decimal(18,2)");
        b.Property(c => c.DiscountAmount).HasColumnType("decimal(18,2)");
        b.Property(c => c.ShippingCost).HasColumnType("decimal(18,2)");
        b.Property(c => c.Total).HasColumnType("decimal(18,2)");

        b.HasIndex(c => c.UserId).IsUnique(); // one cart per user

        b.HasOne(c => c.Coupon)
         .WithMany(cp => cp.Carts)
         .HasForeignKey(c => c.CouponId)
         .OnDelete(DeleteBehavior.SetNull)
         .IsRequired(false);

        b.HasMany(c => c.Items)
         .WithOne(i => i.Cart)
         .HasForeignKey(i => i.CartId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
