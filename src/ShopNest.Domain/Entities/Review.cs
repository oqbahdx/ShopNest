using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Review : AuditableEntity, ISoftDeletable
{
    public Guid         ProductId         { get; set; }
    public Guid         UserId            { get; set; }

    /// <summary>1 – 5 stars.</summary>
    public int          Rating            { get; set; }
    public string?      Title             { get; set; }
    public string?      Comment           { get; set; }
    public ReviewStatus Status            { get; set; } = ReviewStatus.Pending;

    /// <summary>True when the reviewer has a confirmed delivered order for this product.</summary>
    public bool         IsVerifiedPurchase { get; set; } = false;

    public string?      AdminNote         { get; set; }
    public DateTime?    ApprovedAt        { get; set; }
    public Guid?        ApprovedBy        { get; set; }

    // ISoftDeletable
    public bool      IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid?     DeletedBy { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;

    // Domain behaviour
    public void Approve(Guid adminId)
    {
        Status     = ReviewStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = adminId;
    }

    public void Reject(Guid adminId, string note)
    {
        Status    = ReviewStatus.Rejected;
        AdminNote = note;
        ApprovedBy = adminId;
    }
}
