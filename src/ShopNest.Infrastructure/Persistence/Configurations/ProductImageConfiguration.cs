using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        b.HasKey(i => i.Id);
        b.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);
        b.Property(i => i.ThumbnailUrl).HasMaxLength(500);
        b.Property(i => i.AltText).HasMaxLength(250);
        b.HasIndex(i => new { i.ProductId, i.DisplayOrder });
    }
}
