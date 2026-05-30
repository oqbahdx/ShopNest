using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Domain.Common;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IAppDbContext
{
	private readonly IPublisher _publisher;

	public DbSet<Category> Categories { get; set; }

	public DbSet<Product> Products { get; set; }

	public DbSet<ProductImage> ProductImages { get; set; }

	public DbSet<Address> Addresses { get; set; }

	public DbSet<Cart> Carts { get; set; }

	public DbSet<CartItem> CartItems { get; set; }

	public DbSet<Order> Orders { get; set; }

	public DbSet<OrderItem> OrderItems { get; set; }

	public DbSet<Payment> Payments { get; set; }

	public DbSet<Review> Reviews { get; set; }

	public DbSet<Coupon> Coupons { get; set; }

	public DbSet<Notification> Notifications { get; set; }

	public DbSet<RefreshToken> RefreshTokens { get; set; }

	public DbSet<Wishlist> Wishlists { get; set; }

	public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher)
		: base((DbContextOptions)options)
	{
		_publisher = publisher;
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		builder.Entity<AppUser>().ToTable("Users");
		builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
		builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
		builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
		builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
		builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
		builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
		builder.Entity<Address>().HasQueryFilter((Address a) => !a.IsDeleted);
		builder.Entity(delegate(EntityTypeBuilder<AppUser> u)
		{
			u.Property((AppUser x) => x.FirstName).IsRequired().HasMaxLength(100);
			u.Property((AppUser x) => x.LastName).IsRequired().HasMaxLength(100);
			u.Property((AppUser x) => x.AvatarUrl).HasMaxLength(500);
			u.HasIndex((AppUser x) => x.Email).IsUnique();
			u.HasOne((AppUser x) => x.Cart).WithOne().HasForeignKey((Cart c) => c.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			u.HasMany((AppUser x) => x.Orders).WithOne().HasForeignKey((Order o) => o.UserId)
				.OnDelete(DeleteBehavior.Restrict);
			u.HasMany((AppUser x) => x.Addresses).WithOne().HasForeignKey((Address a) => a.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		ProcessEntries();

		List<IHasDomainEvents> aggregates = ChangeTracker.Entries<IHasDomainEvents>()
			.Select((EntityEntry<IHasDomainEvents> e) => e.Entity)
			.Where((IHasDomainEvents e) => e.DomainEvents.Any())
			.ToList();

		List<INotification> domainEvents = aggregates
			.SelectMany((IHasDomainEvents e) => e.DomainEvents)
			.ToList();

		foreach (IHasDomainEvents aggregate in aggregates)
		{
			aggregate.ClearDomainEvents();
		}

		int result = await base.SaveChangesAsync(cancellationToken);

		foreach (INotification domainEvent in domainEvents)
		{
			await _publisher.Publish(domainEvent, cancellationToken);
		}

		return result;
	}

	public override int SaveChanges()
	{
		ProcessEntries();
		return base.SaveChanges();
	}

	private void ProcessEntries()
	{
		DateTime utcNow = DateTime.UtcNow;
		IEnumerable<EntityEntry> enumerable = ChangeTracker.Entries();
		foreach (EntityEntry item in enumerable)
		{
			if (item.Entity is AuditableEntity auditableEntity)
			{
				switch (item.State)
				{
				case EntityState.Added:
					auditableEntity.CreatedAt = utcNow;
					break;
				case EntityState.Modified:
					auditableEntity.UpdatedAt = utcNow;
					item.Property("CreatedAt").IsModified = false;
					break;
				}
			}
			if (item.Entity is AppUser appUser)
			{
				if (item.State == EntityState.Added)
				{
					appUser.CreatedAt = utcNow;
				}
				if (item.State == EntityState.Modified)
				{
					appUser.UpdatedAt = utcNow;
				}
			}
			if (item.State == EntityState.Deleted && item.Entity is ISoftDeletable softDeletable)
			{
				item.State = EntityState.Modified;
				softDeletable.IsDeleted = true;
				softDeletable.DeletedAt = utcNow;
			}
		}
	}
}
