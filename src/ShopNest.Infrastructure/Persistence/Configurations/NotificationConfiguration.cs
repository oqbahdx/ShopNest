using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;
using ShopNest.Application.Common.Identity;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.HasKey(n => n.Id);
        b.Property(n => n.Title).IsRequired().HasMaxLength(200);
        b.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        b.Property(n => n.Data).HasColumnType("nvarchar(max)");

        b.HasIndex(n => n.UserId);
        b.HasIndex(n => new { n.UserId, n.IsRead });
        b.HasIndex(n => n.CreatedAt);

        b.HasOne<AppUser>()
         .WithMany(u => u.Notifications)
         .HasForeignKey(n => n.UserId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
