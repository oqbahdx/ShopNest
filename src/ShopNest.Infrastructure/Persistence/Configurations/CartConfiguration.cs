using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
	public void Configure(EntityTypeBuilder<Cart> b)
	{
		b.HasKey((Cart c) => c.Id);
		b.Property((Cart c) => c.SubTotal).HasColumnType("decimal(18,2)");
		b.Property((Cart c) => c.DiscountAmount).HasColumnType("decimal(18,2)");
		b.Property((Cart c) => c.ShippingCost).HasColumnType("decimal(18,2)");
		b.Property((Cart c) => c.Total).HasColumnType("decimal(18,2)");
		b.HasIndex((Cart c) => c.UserId).IsUnique();
		b.HasOne((Cart c) => c.Coupon).WithMany((Coupon cp) => cp.Carts).HasForeignKey((Cart c) => c.CouponId)
			.OnDelete(DeleteBehavior.SetNull)
			.IsRequired(required: false);
		b.HasMany((Cart c) => c.Items).WithOne((CartItem i) => i.Cart).HasForeignKey((CartItem i) => i.CartId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
