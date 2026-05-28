using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.HasKey(o => o.Id);
        b.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        b.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
        b.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
        b.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
        b.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
        b.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        b.Property(o => o.TrackingNumber).HasMaxLength(100);
        b.Property(o => o.Notes).HasMaxLength(500);
        b.Property(o => o.CancelReason).HasMaxLength(500);

        b.HasIndex(o => o.OrderNumber).IsUnique();
        b.HasIndex(o => o.UserId);
        b.HasIndex(o => o.Status);
        b.HasIndex(o => o.CreatedAt);

        b.HasOne(o => o.ShippingAddress)
         .WithMany(a => a.Orders)
         .HasForeignKey(o => o.ShippingAddressId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.Coupon)
         .WithMany(c => c.Orders)
         .HasForeignKey(o => o.CouponId)
         .OnDelete(DeleteBehavior.SetNull)
         .IsRequired(false);

        b.HasOne(o => o.Payment)
         .WithOne(p => p.Order)
         .HasForeignKey<Payment>(p => p.OrderId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(o => o.Items)
         .WithOne(i => i.Order)
         .HasForeignKey(i => i.OrderId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(o => !o.IsDeleted);
    }
}
