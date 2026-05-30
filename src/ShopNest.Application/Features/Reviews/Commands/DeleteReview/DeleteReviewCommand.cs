using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Commands.DeleteReview;

public sealed record DeleteReviewCommand(
    Guid ReviewId,
    bool IsAdmin = false
) : IRequest<Result>;

public sealed class DeleteReviewCommandHandler
    : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteReviewCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteReviewCommand cmd, CancellationToken ct)
    {
        var review = await _db.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == cmd.ReviewId, ct);

        if (review is null)
            return Result.Failure("Review not found.", ErrorCodes.NOT_FOUND);

        // Author or Admin can delete
        if (!cmd.IsAdmin && review.UserId != _currentUser.UserId)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        var wasApproved = review.Status == ReviewStatus.Approved;
        var productId = review.ProductId;

        // Soft-delete via ISoftDeletable
        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync(ct);

        // Recalculate rating only if the deleted review was Approved
        // (Pending/Rejected reviews don't affect the public rating)
        if (wasApproved)
        {
            var product = await _db.Products
                .FindAsync(new object[] { productId }, ct);

            if (product is not null)
            {
                var remaining = await _db.Reviews
                    .Where(r =>
                        r.ProductId == productId &&
                        r.Status == ReviewStatus.Approved)
                    .ToListAsync(ct);

                product.RecalculateRating(remaining);
                await _db.SaveChangesAsync(ct);
            }
        }

        return Result.Success();
    }
}