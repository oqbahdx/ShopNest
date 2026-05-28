using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Infrastructure.Persistence;

/// <summary>
/// Primary EF Core DbContext.
/// Inherits IdentityDbContext so ASP.NET Identity tables are included
/// in the same schema as the rest of the application.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── DbSets ────────────────────────────────────────────────────────────────
    public DbSet<Category>      Categories    { get; set; }
    public DbSet<Product>       Products      { get; set; }
    public DbSet<ProductImage>  ProductImages { get; set; }
    public DbSet<Address>       Addresses     { get; set; }
    public DbSet<Cart>          Carts         { get; set; }
    public DbSet<CartItem>      CartItems     { get; set; }
    public DbSet<Order>         Orders        { get; set; }
    public DbSet<OrderItem>     OrderItems    { get; set; }
    public DbSet<Payment>       Payments      { get; set; }
    public DbSet<Review>        Reviews       { get; set; }
    public DbSet<Coupon>        Coupons       { get; set; }
    public DbSet<Notification>  Notifications { get; set; }
    public DbSet<RefreshToken>  RefreshTokens { get; set; }

    // ── Model configuration ───────────────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // must be called first for Identity tables

        // Apply all IEntityTypeConfiguration<T> classes from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Rename Identity tables to cleaner names
        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        // AppUser navigation configurations (done here because AppUser is in Infrastructure)
        builder.Entity<AppUser>(u =>
        {
            u.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            u.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            u.Property(x => x.AvatarUrl).HasMaxLength(500);
            u.HasIndex(x => x.Email).IsUnique();

            u.HasOne(x => x.Cart)
             .WithOne()
             .HasForeignKey<Cart>(c => c.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            u.HasMany(x => x.Orders)
             .WithOne()
             .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            u.HasMany(x => x.Addresses)
             .WithOne()
             .HasForeignKey(a => a.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // ── Audit + Soft-delete on SaveChanges ────────────────────────────────────
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        ProcessEntries();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ProcessEntries();
        return base.SaveChanges();
    }

    private void ProcessEntries()
    {
        var now    = DateTime.UtcNow;
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            // ── Audit timestamps ──────────────────────────────────────────────
            if (entry.Entity is AuditableEntity auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAt = now;
                        break;
                    case EntityState.Modified:
                        auditable.UpdatedAt = now;
                        // Prevent accidental overwrites of CreatedAt on update
                        entry.Property(nameof(AuditableEntity.CreatedAt)).IsModified = false;
                        break;
                }
            }

            // AppUser audit (not derived from AuditableEntity)
            if (entry.Entity is AppUser user)
            {
                if (entry.State == EntityState.Added)
                    user.CreatedAt = now;
                if (entry.State == EntityState.Modified)
                    user.UpdatedAt = now;
            }

            // ── Soft-delete interception ──────────────────────────────────────
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletable softDeletable)
            {
                entry.State             = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAt = now;
            }
        }
    }
}
