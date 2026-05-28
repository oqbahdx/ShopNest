using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Code).IsRequired().HasMaxLength(50);
        b.Property(c => c.Description).HasMaxLength(300);
        b.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
        b.Property(c => c.MinimumOrderAmount).HasColumnType("decimal(18,2)");
        b.Property(c => c.MaximumDiscountAmount).HasColumnType("decimal(18,2)");

        b.HasIndex(c => c.Code).IsUnique();
        b.HasIndex(c => c.IsActive);
        b.HasIndex(c => c.ExpiresAt);
    }
}
