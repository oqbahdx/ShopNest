using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> b)
	{
		b.HasKey((Product p) => p.Id);
		b.Property((Product p) => p.Name).IsRequired().HasMaxLength(200);
		b.Property((Product p) => p.Slug).IsRequired().HasMaxLength(250);
		b.Property((Product p) => p.ShortDescription).HasMaxLength(500);
		b.Property((Product p) => p.SKU).IsRequired().HasMaxLength(100);
		b.Property((Product p) => p.Barcode).HasMaxLength(100);
		b.Property((Product p) => p.Price).HasColumnType("decimal(18,2)");
		b.Property((Product p) => p.CompareAtPrice).HasColumnType("decimal(18,2)");
		b.Property((Product p) => p.CostPrice).HasColumnType("decimal(18,2)");
		b.Property((Product p) => p.Weight).HasColumnType("decimal(10,3)");
		b.Property((Product p) => p.AverageRating).HasColumnType("decimal(3,2)");
		b.HasIndex((Product p) => p.Slug).IsUnique();
		b.HasIndex((Product p) => p.SKU).IsUnique();
		b.HasIndex((Product p) => p.CategoryId);
		b.HasIndex((Product p) => new { p.IsActive, p.IsDeleted, p.CreatedAt });
		b.HasOne((Product p) => p.Category).WithMany((Category c) => c.Products).HasForeignKey((Product p) => p.CategoryId)
			.OnDelete(DeleteBehavior.Restrict);
		b.HasMany((Product p) => p.Images).WithOne((ProductImage i) => i.Product).HasForeignKey((ProductImage i) => i.ProductId)
			.OnDelete(DeleteBehavior.Cascade);
		b.HasMany((Product p) => p.Reviews).WithOne((Review r) => r.Product).HasForeignKey((Review r) => r.ProductId)
			.OnDelete(DeleteBehavior.Cascade);
		b.HasQueryFilter((Product p) => !p.IsDeleted);
	}
}
