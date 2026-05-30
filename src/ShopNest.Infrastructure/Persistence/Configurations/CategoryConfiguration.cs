using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
	public void Configure(EntityTypeBuilder<Category> b)
	{
		b.HasKey((Category c) => c.Id);
		b.Property((Category c) => c.Name).IsRequired().HasMaxLength(150);
		b.Property((Category c) => c.Slug).IsRequired().HasMaxLength(180);
		b.Property((Category c) => c.Description).HasMaxLength(1000);
		b.Property((Category c) => c.ImageUrl).HasMaxLength(500);
		b.HasIndex((Category c) => c.Slug).IsUnique();
		b.HasIndex((Category c) => c.IsActive);
		b.HasOne((Category c) => c.ParentCategory).WithMany((Category c) => c.SubCategories).HasForeignKey((Category c) => c.ParentCategoryId)
			.OnDelete(DeleteBehavior.Restrict)
			.IsRequired(required: false);
		b.HasQueryFilter((Category c) => !c.IsDeleted);
	}
}
