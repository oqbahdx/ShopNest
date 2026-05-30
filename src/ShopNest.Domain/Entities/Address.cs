using System;
using System.Collections.Generic;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

/// <summary>
/// A saved shipping address belonging to a user.
/// Max 10 per user, soft-deletable, one may be flagged IsDefault.
/// Snapshotted onto orders at checkout time (Phase 5 upgrade of PlaceOrderCommand).
/// </summary>
public sealed class Address : AuditableEntity, ISoftDeletable
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Address()
    {
    }

    public static Address Create(
        Guid userId,
        string fullName,
        string line1,
        string? line2,
        string city,
        string state,
        string postalCode,
        string country,
        string? phone) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        FullName = fullName,
        Street = line1,
        City = city,
        State = state,
        PostalCode = postalCode,
        Country = country,
        Phone = phone ?? string.Empty
    };

    public void Update(
        string fullName,
        string line1,
        string? line2,
        string city,
        string state,
        string postalCode,
        string country,
        string? phone)
    {
        FullName = fullName;
        Street = line1;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        Phone = phone ?? string.Empty;
    }

    public void SetDefault(bool isDefault) => IsDefault = isDefault;

    // ISoftDeletable — AppDbContext intercepts Remove() calls
    public void Delete() => IsDeleted = true;
}
