namespace ShopNest.Application.Features.Users.DTOs;

public sealed record AddressDto(
    Guid Id,
    string FullName,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country,
    string? Phone,
    bool IsDefault
);