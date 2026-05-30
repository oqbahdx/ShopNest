using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
	public void Configure(EntityTypeBuilder<CartItem> b)
	{
		b.HasKey((CartItem i) => i.Id);
		b.Property((CartItem i) => i.UnitPrice).HasColumnType("decimal(18,2)");
		b.Property((CartItem i) => i.TotalPrice).HasColumnType("decimal(18,2)");
		b.HasIndex((CartItem i) => new { i.CartId, i.ProductId }).IsUnique();
		b.HasOne((CartItem i) => i.Product).WithMany((Product p) => p.CartItems).HasForeignKey((CartItem i) => i.ProductId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
