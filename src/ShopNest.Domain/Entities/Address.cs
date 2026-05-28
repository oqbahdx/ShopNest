using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class Address : AuditableEntity, ISoftDeletable
{
    public Guid   UserId     { get; set; }
    public string FullName   { get; set; } = string.Empty;
    public string Phone      { get; set; } = string.Empty;
    public string Street     { get; set; } = string.Empty;
    public string City       { get; set; } = string.Empty;
    public string State      { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country    { get; set; } = string.Empty;
    public bool   IsDefault  { get; set; } = false;

    // ISoftDeletable
    public bool      IsDeleted  { get; set; } = false;
    public DateTime? DeletedAt  { get; set; }
    public Guid?     DeletedBy  { get; set; }

    // Navigation
    public ICollection<Order> Orders { get; set; } = [];
}
