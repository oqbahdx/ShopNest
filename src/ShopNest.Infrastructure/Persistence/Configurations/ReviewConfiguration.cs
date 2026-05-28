using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;
using ShopNest.Application.Common.Identity;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> b)
    {
        b.HasKey(r => r.Id);
        b.Property(r => r.Title).HasMaxLength(200);
        b.Property(r => r.Comment).HasMaxLength(2000);
        b.Property(r => r.AdminNote).HasMaxLength(500);

        // One review per user per product
        b.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
        b.HasIndex(r => r.ProductId);
        b.HasIndex(r => r.Status);

        b.HasOne<AppUser>()
         .WithMany(u => u.Reviews)
         .HasForeignKey(r => r.UserId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(r => !r.IsDeleted);
    }
}
