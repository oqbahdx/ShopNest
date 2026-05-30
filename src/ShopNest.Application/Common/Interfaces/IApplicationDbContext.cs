using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<Category> Categories { get; }

	DbSet<Product> Products { get; }

	DbSet<ProductImage> ProductImages { get; }

	DbSet<Address> Addresses { get; }

	DbSet<Cart> Carts { get; }

	DbSet<CartItem> CartItems { get; }

	DbSet<Order> Orders { get; }

	DbSet<OrderItem> OrderItems { get; }

	DbSet<Payment> Payments { get; }

	DbSet<Review> Reviews { get; }

	DbSet<Coupon> Coupons { get; }

	DbSet<Notification> Notifications { get; }

	DbSet<RefreshToken> RefreshTokens { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
