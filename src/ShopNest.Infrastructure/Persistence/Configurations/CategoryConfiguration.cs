using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Name).IsRequired().HasMaxLength(150);
        b.Property(c => c.Slug).IsRequired().HasMaxLength(180);
        b.Property(c => c.Description).HasMaxLength(1000);
        b.Property(c => c.ImageUrl).HasMaxLength(500);

        b.HasIndex(c => c.Slug).IsUnique();
        b.HasIndex(c => c.IsActive);

        b.HasOne(c => c.ParentCategory)
         .WithMany(c => c.SubCategories)
         .HasForeignKey(c => c.ParentCategoryId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        b.HasQueryFilter(c => !c.IsDeleted);
    }
}
