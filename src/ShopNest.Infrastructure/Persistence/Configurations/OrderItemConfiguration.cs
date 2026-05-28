using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.HasKey(i => i.Id);
        b.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        b.Property(i => i.ProductImageUrl).HasMaxLength(500);
        b.Property(i => i.ProductSKU).IsRequired().HasMaxLength(100);
        b.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        b.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");

        b.HasIndex(i => i.OrderId);

        // Soft FK — product may be deleted but order item must remain
        b.HasOne(i => i.Product)
         .WithMany(p => p.OrderItems)
         .HasForeignKey(i => i.ProductId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
