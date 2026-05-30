using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShopNest.Application.Common.Identity;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Common.Interfaces;

public interface IAppDbContext : IApplicationDbContext
{
    DbSet<AppUser> Users { get; }

    DbSet<Wishlist> Wishlists { get; }

    DatabaseFacade Database { get; }
}
