using MediatR;
using ShopNest.Application.Features.Reviews.DTOs;

namespace ShopNest.Application.Features.Reviews.Queries.GetProductReviews;

/// <summary>
/// Public endpoint — returns approved reviews only.
/// Customers see the IsVerifiedPurchase badge.
/// </summary>
public sealed record GetProductReviewsQuery(
    Guid ProductId,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "date", // date | rating
    string SortOrder = "desc"
) : IRequest<Result<PagedResult<ReviewDto>>>;