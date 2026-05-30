using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Application.Common.Identity;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
	public void Configure(EntityTypeBuilder<Notification> b)
	{
		b.HasKey((Notification n) => n.Id);
		b.Property((Notification n) => n.Title).IsRequired().HasMaxLength(200);
		b.Property((Notification n) => n.Message).IsRequired().HasMaxLength(1000);
		b.Property((Notification n) => n.Data).HasColumnType("nvarchar(max)");
		b.HasIndex((Notification n) => n.UserId);
		b.HasIndex((Notification n) => new { n.UserId, n.IsRead });
		b.HasIndex((Notification n) => n.CreatedAt);
		b.HasOne<AppUser>().WithMany((AppUser u) => u.Notifications).HasForeignKey((Notification n) => n.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
