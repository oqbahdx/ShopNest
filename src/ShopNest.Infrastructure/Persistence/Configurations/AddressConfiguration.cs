using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
	public void Configure(EntityTypeBuilder<Address> b)
	{
		b.HasKey((Address a) => a.Id);
		b.Property((Address a) => a.FullName).IsRequired().HasMaxLength(150);
		b.Property((Address a) => a.Phone).IsRequired().HasMaxLength(30);
		b.Property((Address a) => a.Street).IsRequired().HasMaxLength(250);
		b.Property((Address a) => a.City).IsRequired().HasMaxLength(100);
		b.Property((Address a) => a.State).HasMaxLength(100);
		b.Property((Address a) => a.PostalCode).IsRequired().HasMaxLength(20);
		b.Property((Address a) => a.Country).IsRequired().HasMaxLength(100);
		b.HasIndex((Address a) => a.UserId);
		b.HasQueryFilter((Address a) => !a.IsDeleted);
	}
}
