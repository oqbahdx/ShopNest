using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.DTOs;

public sealed record ReviewDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    Guid UserId,
    string UserDisplayName,
    int Rating,
    string Title,
    string Comment,
    bool IsVerifiedPurchase,
    ReviewStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record AdminReviewDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    Guid UserId,
    string UserEmail,
    int Rating,
    string Title,
    string Comment,
    bool IsVerifiedPurchase,
    ReviewStatus Status,
    string? RejectionNote,
    DateTime CreatedAt
);

public sealed record CanReviewDto(
    bool CanReview,
    string? Reason // null when CanReview = true
);