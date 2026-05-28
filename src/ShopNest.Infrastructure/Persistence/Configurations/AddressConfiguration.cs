using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> b)
    {
        b.HasKey(a => a.Id);
        b.Property(a => a.FullName).IsRequired().HasMaxLength(150);
        b.Property(a => a.Phone).IsRequired().HasMaxLength(30);
        b.Property(a => a.Street).IsRequired().HasMaxLength(250);
        b.Property(a => a.City).IsRequired().HasMaxLength(100);
        b.Property(a => a.State).HasMaxLength(100);
        b.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
        b.Property(a => a.Country).IsRequired().HasMaxLength(100);

        b.HasIndex(a => a.UserId);
        b.HasQueryFilter(a => !a.IsDeleted);
    }
}
