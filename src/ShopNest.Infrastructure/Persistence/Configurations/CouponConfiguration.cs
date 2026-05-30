using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
	public void Configure(EntityTypeBuilder<Coupon> b)
	{
		b.HasKey((Coupon c) => c.Id);
		b.Property((Coupon c) => c.Code).IsRequired().HasMaxLength(50);
		b.Property((Coupon c) => c.Description).HasMaxLength(300);
		b.Property((Coupon c) => c.DiscountValue).HasColumnType("decimal(18,2)");
		b.Property((Coupon c) => c.MinimumOrderAmount).HasColumnType("decimal(18,2)");
		b.Property((Coupon c) => c.MaximumDiscountAmount).HasColumnType("decimal(18,2)");
		b.Ignore((Coupon c) => c.UsedCount);
		b.Property((Coupon c) => c.UsageCount).HasColumnName(nameof(Coupon.UsedCount));
		b.HasIndex((Coupon c) => c.Code).IsUnique();
		b.HasIndex((Coupon c) => c.IsActive);
		b.HasIndex((Coupon c) => c.ExpiresAt);
	}
}
