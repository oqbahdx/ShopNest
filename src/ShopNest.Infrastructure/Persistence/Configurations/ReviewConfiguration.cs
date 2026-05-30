using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Application.Common.Identity;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure(EntityTypeBuilder<Review> b)
	{
		b.HasKey((Review r) => r.Id);
		b.Property((Review r) => r.Title).HasMaxLength(200);
		b.Property((Review r) => r.Comment).HasMaxLength(2000);
		b.Property((Review r) => r.AdminNote).HasMaxLength(500);
		b.HasIndex((Review r) => new { r.UserId, r.ProductId }).IsUnique();
		b.HasIndex((Review r) => r.ProductId);
		b.HasIndex((Review r) => r.Status);
		b.HasOne<AppUser>().WithMany((AppUser u) => u.Reviews).HasForeignKey((Review r) => r.UserId)
			.OnDelete(DeleteBehavior.Restrict);
		b.HasQueryFilter((Review r) => !r.IsDeleted);
	}
}
