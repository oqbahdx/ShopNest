namespace ShopNest.Application.Features.Auth.DTOs;

public sealed record CurrentUserDto(
    Guid   Id,
    string FirstName,
    string LastName,
    string Email,
    string? AvatarUrl,
    IReadOnlyList<string> Roles);
