using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
	public void Configure(EntityTypeBuilder<OrderItem> b)
	{
		b.HasKey((OrderItem i) => i.Id);
		b.Property((OrderItem i) => i.ProductName).IsRequired().HasMaxLength(200);
		b.Property((OrderItem i) => i.ProductImageUrl).HasMaxLength(500);
		b.Property((OrderItem i) => i.ProductSKU).IsRequired().HasMaxLength(100);
		b.Property((OrderItem i) => i.UnitPrice).HasColumnType("decimal(18,2)");
		b.Property((OrderItem i) => i.TotalPrice).HasColumnType("decimal(18,2)");
		b.HasIndex((OrderItem i) => i.OrderId);
		b.HasOne((OrderItem i) => i.Product).WithMany((Product p) => p.OrderItems).HasForeignKey((OrderItem i) => i.ProductId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
