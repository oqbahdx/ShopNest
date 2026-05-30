using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Users.DTOs;

public sealed record NotificationDto(
    Guid Id,
    NotificationType Type,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);
