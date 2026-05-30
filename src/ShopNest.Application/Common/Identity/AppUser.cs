using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Common.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string? AvatarUrl { get; set; }

	public bool IsActive { get; set; } = true;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime? UpdatedAt { get; set; }

	public string FullName => (FirstName + " " + LastName).Trim();

	public Cart? Cart { get; set; }

	public ICollection<Order> Orders { get; set; } = new List<Order>();

	public ICollection<Address> Addresses { get; set; } = new List<Address>();

	public ICollection<Review> Reviews { get; set; } = new List<Review>();

	public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

	public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
