namespace ShopNest.Application.Features.Users.DTOs;

public sealed record ProfileDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? AvatarUrl,
    bool IsEmailConfirmed,
    DateTime CreatedAt
);