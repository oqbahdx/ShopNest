using FluentValidation;
using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Commands.RejectReview;

public sealed record RejectReviewCommand(
    Guid ReviewId,
    string Note
) : IRequest<Result>;

public sealed class RejectReviewCommandValidator
    : FluentValidation.AbstractValidator<RejectReviewCommand>
{
    public RejectReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("Review ID is required.");

        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("A rejection note is required.")
            .MaximumLength(500);
    }
}

public sealed class RejectReviewCommandHandler
    : IRequestHandler<RejectReviewCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RejectReviewCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        RejectReviewCommand cmd, CancellationToken ct)
    {
        var adminId = _currentUser.UserId;
        if (adminId is null)
            return Result.Failure("Authentication required.", ErrorCodes.FORBIDDEN);

        var review = await _db.Reviews
            .FirstOrDefaultAsync(r => r.Id == cmd.ReviewId, ct);

        if (review is null)
            return Result.Failure("Review not found.", ErrorCodes.NOT_FOUND);

        if (review.Status != ReviewStatus.Pending)
            return Result.Failure(
                "Only Pending reviews can be rejected.",
                ErrorCodes.CONFLICT);

        // Reject via domain method — does NOT affect product rating
        review.Reject(adminId.Value, cmd.Note);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
