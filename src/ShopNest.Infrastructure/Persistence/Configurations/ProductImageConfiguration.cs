using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
	public void Configure(EntityTypeBuilder<ProductImage> b)
	{
		b.HasKey((ProductImage i) => i.Id);
		b.Property((ProductImage i) => i.ImageUrl).IsRequired().HasMaxLength(500);
		b.Property((ProductImage i) => i.ThumbnailUrl).HasMaxLength(500);
		b.Property((ProductImage i) => i.AltText).HasMaxLength(250);
		b.HasIndex((ProductImage i) => new { i.ProductId, i.DisplayOrder });
	}
}
