using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> b)
	{
		b.HasKey((Order o) => o.Id);
		b.Property((Order o) => o.OrderNumber).IsRequired().HasMaxLength(50);
		b.Property((Order o) => o.SubTotal).HasColumnType("decimal(18,2)");
		b.Property((Order o) => o.DiscountAmount).HasColumnType("decimal(18,2)");
		b.Property((Order o) => o.ShippingCost).HasColumnType("decimal(18,2)");
		b.Property((Order o) => o.TaxAmount).HasColumnType("decimal(18,2)");
		b.Property((Order o) => o.TotalAmount).HasColumnType("decimal(18,2)");
		b.Property((Order o) => o.TrackingNumber).HasMaxLength(100);
		b.Property((Order o) => o.Notes).HasMaxLength(500);
		b.Property((Order o) => o.CancelReason).HasMaxLength(500);
		b.HasIndex((Order o) => o.OrderNumber).IsUnique();
		b.HasIndex((Order o) => o.UserId);
		b.HasIndex((Order o) => o.Status);
		b.HasIndex((Order o) => o.CreatedAt);
		b.HasOne((Order o) => o.ShippingAddress).WithMany((Address a) => a.Orders).HasForeignKey((Order o) => o.ShippingAddressId)
			.OnDelete(DeleteBehavior.Restrict);
		b.HasOne((Order o) => o.Coupon).WithMany((Coupon c) => c.Orders).HasForeignKey((Order o) => o.CouponId)
			.OnDelete(DeleteBehavior.SetNull)
			.IsRequired(required: false);
		b.HasOne((Order o) => o.Payment).WithOne((Payment p) => p.Order).HasForeignKey((Payment p) => p.OrderId)
			.OnDelete(DeleteBehavior.Cascade);
		b.HasMany((Order o) => o.Items).WithOne((OrderItem i) => i.Order).HasForeignKey((OrderItem i) => i.OrderId)
			.OnDelete(DeleteBehavior.Cascade);
		b.HasQueryFilter((Order o) => !o.IsDeleted);
	}
}
