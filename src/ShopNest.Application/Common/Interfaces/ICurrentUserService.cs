namespace ShopNest.Application.Common.Interfaces;

/// <summary>Provides the identity of the current HTTP caller. Null when anonymous.</summary>
public interface ICurrentUserService
{
    Guid?   UserId          { get; }
    string? Email           { get; }
    bool    IsAuthenticated { get; }
    string? IpAddress       { get; }
}
