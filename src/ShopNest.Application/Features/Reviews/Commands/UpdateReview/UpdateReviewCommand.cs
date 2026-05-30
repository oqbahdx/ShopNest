using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Commands.UpdateReview;

public sealed record UpdateReviewCommand(
    Guid ReviewId,
    int Rating,
    string Title,
    string Comment
) : IRequest<Result>;

public sealed class UpdateReviewCommandHandler
    : IRequestHandler<UpdateReviewCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateReviewCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateReviewCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result.Failure("Authentication required.", ErrorCodes.FORBIDDEN);

        var review = await _db.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == cmd.ReviewId, ct);

        if (review is null)
            return Result.Failure("Review not found.", ErrorCodes.NOT_FOUND);

        // Only the author can update their review
        if (review.UserId != userId.Value)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        var wasApproved = review.Status == ReviewStatus.Approved;

        // Update resets status to Pending — requires re-moderation
        review.Update(cmd.Rating, cmd.Title, cmd.Comment);

        // If the review was previously Approved, recalculate the product rating
        // because the rating value may have changed
        if (wasApproved)
        {
            var approvedReviews = await _db.Reviews
                .Where(r =>
                    r.ProductId == review.ProductId &&
                    r.Status == ReviewStatus.Approved &&
                    r.Id != review.Id) // exclude this review — it's now Pending
                .ToListAsync(ct);

            review.Product.RecalculateRating(approvedReviews);
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
