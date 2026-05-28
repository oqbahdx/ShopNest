using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid      UserId           { get; set; }

    /// <summary>SHA-256 hash of the raw token — never store plaintext.</summary>
    public string    TokenHash        { get; set; } = string.Empty;

    public DateTime  ExpiresAt        { get; set; }
    public bool      IsRevoked        { get; set; } = false;
    public DateTime? RevokedAt        { get; set; }
    public string?   RevokedByIp      { get; set; }
    public string?   ReplacedByToken  { get; set; }  // token hash of its replacement
    public string?   CreatedByIp      { get; set; }
    public DateTime  CreatedAt        { get; set; } = DateTime.UtcNow;

    // Domain behaviour
    public bool IsActive  => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public void Revoke(string? ip = null, string? replacedBy = null)
    {
        IsRevoked       = true;
        RevokedAt       = DateTime.UtcNow;
        RevokedByIp     = ip;
        ReplacedByToken = replacedBy;
    }
}
