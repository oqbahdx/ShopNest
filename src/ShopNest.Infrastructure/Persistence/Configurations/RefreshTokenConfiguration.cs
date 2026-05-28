using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;
using ShopNest.Application.Common.Identity;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.HasKey(t => t.Id);
        b.Property(t => t.TokenHash).IsRequired().HasMaxLength(512);
        b.Property(t => t.CreatedByIp).HasMaxLength(50);
        b.Property(t => t.RevokedByIp).HasMaxLength(50);
        b.Property(t => t.ReplacedByToken).HasMaxLength(512);

        b.HasIndex(t => t.TokenHash);
        b.HasIndex(t => t.UserId);
        b.HasIndex(t => t.ExpiresAt);

        b.HasOne<AppUser>()
         .WithMany(u => u.RefreshTokens)
         .HasForeignKey(t => t.UserId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
