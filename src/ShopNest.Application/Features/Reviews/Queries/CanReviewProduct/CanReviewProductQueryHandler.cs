using MediatR;
using ShopNest.Application.Features.Reviews.DTOs;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Queries.CanReviewProduct;

public sealed record CanReviewProductQuery(Guid ProductId)
    : IRequest<Result<CanReviewDto>>;

public sealed class CanReviewProductQueryHandler
    : IRequestHandler<CanReviewProductQuery, Result<CanReviewDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CanReviewProductQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CanReviewDto>> Handle(
        CanReviewProductQuery qry, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        // Check 1: already has a review for this product?
        var hasReview = await _db.Reviews
            .AnyAsync(r =>
                r.ProductId == qry.ProductId &&
                r.UserId == userId, ct);

        if (hasReview)
            return Result<CanReviewDto>.Success(new CanReviewDto(
                CanReview: false,
                Reason: "You have already reviewed this product."));

        // Check 2: has a delivered order containing the product?
        var hasDeliveredOrder = await _db.OrderItems
            .AnyAsync(oi =>
                oi.ProductId == qry.ProductId &&
                oi.Order.UserId == userId &&
                oi.Order.Status == OrderStatus.Delivered, ct);

        if (!hasDeliveredOrder)
            return Result<CanReviewDto>.Success(new CanReviewDto(
                CanReview: false,
                Reason: "You can only review products from a delivered order."));

        return Result<CanReviewDto>.Success(
            new CanReviewDto(CanReview: true, Reason: null));
    }
}