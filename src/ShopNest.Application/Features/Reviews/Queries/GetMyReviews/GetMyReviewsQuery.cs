using MediatR;
using ShopNest.Application.Features.Reviews.DTOs;

namespace ShopNest.Application.Features.Reviews.Queries.GetMyReviews;

public sealed record GetMyReviewsQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<ReviewDto>>>;

public sealed class GetMyReviewsQueryHandler
    : IRequestHandler<GetMyReviewsQuery, Result<PagedResult<ReviewDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyReviewsQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ReviewDto>>> Handle(
        GetMyReviewsQuery qry, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<PagedResult<ReviewDto>>.Failure(
                "Authentication required.", ErrorCodes.FORBIDDEN);

        var q = _db.Reviews
            .AsNoTracking()
            .Include(r => r.Product)
            .Where(r => r.UserId == userId.Value);

        var total = await q.CountAsync(ct);

        var reviews = await q
            .OrderByDescending(r => r.CreatedAt)
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .ToListAsync(ct);

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId.Value, ct);
        var displayName = user is null
            ? "Me"
            : GetDisplayName(user.FirstName, user.LastName, user.UserName);

        var items = reviews.Select(r => new ReviewDto(
            Id: r.Id,
            ProductId: r.ProductId,
            ProductName: r.Product.Name,
            UserId: r.UserId,
            UserDisplayName: displayName,
            Rating: r.Rating,
            Title: r.Title ?? string.Empty,
            Comment: r.Comment ?? string.Empty,
            IsVerifiedPurchase: r.IsVerifiedPurchase,
            Status: r.Status,
            CreatedAt: r.CreatedAt,
            UpdatedAt: r.UpdatedAt
        )).ToList();

        return Result<PagedResult<ReviewDto>>.Success(
            PagedResult<ReviewDto>.Create(
                items, qry.Page, qry.PageSize, total));
    }

    private static string GetDisplayName(string firstName, string lastName, string? userName)
    {
        var fullName = $"{firstName} {lastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName)
            ? userName ?? "Me"
            : fullName;
    }
}
