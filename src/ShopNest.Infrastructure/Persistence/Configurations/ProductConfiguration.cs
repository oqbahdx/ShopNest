using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.Property(p => p.Slug).IsRequired().HasMaxLength(250);
        b.Property(p => p.ShortDescription).HasMaxLength(500);
        b.Property(p => p.SKU).IsRequired().HasMaxLength(100);
        b.Property(p => p.Barcode).HasMaxLength(100);
        b.Property(p => p.Price).HasColumnType("decimal(18,2)");
        b.Property(p => p.CompareAtPrice).HasColumnType("decimal(18,2)");
        b.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
        b.Property(p => p.Weight).HasColumnType("decimal(10,3)");
        b.Property(p => p.AverageRating).HasColumnType("decimal(3,2)");

        b.HasIndex(p => p.Slug).IsUnique();
        b.HasIndex(p => p.SKU).IsUnique();
        b.HasIndex(p => p.CategoryId);
        b.HasIndex(p => new { p.IsActive, p.IsDeleted, p.CreatedAt });

        b.HasOne(p => p.Category)
         .WithMany(c => c.Products)
         .HasForeignKey(p => p.CategoryId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(p => p.Images)
         .WithOne(i => i.Product)
         .HasForeignKey(i => i.ProductId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.Reviews)
         .WithOne(r => r.Product)
         .HasForeignKey(r => r.ProductId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(p => !p.IsDeleted);
    }
}
