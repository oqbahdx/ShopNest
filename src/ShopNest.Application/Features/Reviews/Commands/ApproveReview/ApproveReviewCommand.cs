using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Commands.ApproveReview;

public sealed record ApproveReviewCommand(
    Guid ReviewId
) : IRequest<Result>;

public sealed class ApproveReviewCommandHandler
    : IRequestHandler<ApproveReviewCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ApproveReviewCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ApproveReviewCommand cmd, CancellationToken ct)
    {
        var adminId = _currentUser.UserId;
        if (adminId is null)
            return Result.Failure("Authentication required.", ErrorCodes.FORBIDDEN);

        var review = await _db.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == cmd.ReviewId, ct);

        if (review is null)
            return Result.Failure("Review not found.", ErrorCodes.NOT_FOUND);

        if (review.Status != ReviewStatus.Pending)
            return Result.Failure(
                $"Only Pending reviews can be approved. " +
                $"Current status: {review.Status}.",
                ErrorCodes.CONFLICT);

        // 1. Approve via domain method
        review.Approve(adminId.Value);

        // 2. Recalculate product rating — MUST include this review now
        var allApproved = await _db.Reviews
            .Where(r =>
                r.ProductId == review.ProductId &&
                r.Status == ReviewStatus.Approved)
            .ToListAsync(ct);

        // The review is not saved yet so it won't appear in the query above —
        // add it manually so RecalculateRating sees the full approved set
        allApproved.Add(review);
        review.Product.RecalculateRating(allApproved);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
