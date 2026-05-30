using MediatR;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateReviewCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<Guid>.Failure("Authentication required.", ErrorCodes.FORBIDDEN);

        // 1. Verify product exists
        var productExists = await _db.Products
            .AnyAsync(p => p.Id == cmd.ProductId, ct);

        if (!productExists)
            return Result<Guid>.Failure(
                "Product not found.", ErrorCodes.NOT_FOUND);

        // 2. One review per user per product
        var alreadyReviewed = await _db.Reviews
            .AnyAsync(r =>
                r.ProductId == cmd.ProductId &&
                r.UserId == userId.Value, ct);

        if (alreadyReviewed)
            return Result<Guid>.Failure(
                "You have already reviewed this product.",
                ErrorCodes.CONFLICT);

        // 3. Check for verified purchase
        // User must have a Delivered order containing this product
        var isVerifiedPurchase = await _db.OrderItems
            .AnyAsync(oi =>
                oi.ProductId == cmd.ProductId &&
                oi.Order.UserId == userId.Value &&
                oi.Order.Status == OrderStatus.Delivered, ct);

        if (!isVerifiedPurchase)
            return Result<Guid>.Failure(
                "You can only review products from a delivered order.",
                ErrorCodes.CONFLICT);

        // 4. Create review — starts Pending, awaiting admin approval
        var review = Review.Create(
            productId: cmd.ProductId,
            userId: userId.Value,
            rating: cmd.Rating,
            title: cmd.Title,
            comment: cmd.Comment,
            isVerifiedPurchase: true
        );

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(review.Id);
    }
}
