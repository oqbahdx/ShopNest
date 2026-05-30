using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Application.Common.Identity;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> b)
	{
		b.HasKey((RefreshToken t) => t.Id);
		b.Property((RefreshToken t) => t.TokenHash).IsRequired().HasMaxLength(512);
		b.Property((RefreshToken t) => t.CreatedByIp).HasMaxLength(50);
		b.Property((RefreshToken t) => t.RevokedByIp).HasMaxLength(50);
		b.Property((RefreshToken t) => t.ReplacedByToken).HasMaxLength(512);
		b.HasIndex((RefreshToken t) => t.TokenHash);
		b.HasIndex((RefreshToken t) => t.UserId);
		b.HasIndex((RefreshToken t) => t.ExpiresAt);
		b.HasOne<AppUser>().WithMany((AppUser u) => u.RefreshTokens).HasForeignKey((RefreshToken t) => t.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
