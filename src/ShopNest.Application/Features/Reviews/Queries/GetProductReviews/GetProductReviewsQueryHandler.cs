using MediatR;
using ShopNest.Application.Features.Reviews.DTOs;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Queries.GetProductReviews;

public sealed class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, Result<PagedResult<ReviewDto>>>
{
    private readonly IAppDbContext _db;

    public GetProductReviewsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PagedResult<ReviewDto>>> Handle(
        GetProductReviewsQuery qry, CancellationToken ct)
    {
        // Public: approved reviews only
        var q = _db.Reviews
            .AsNoTracking()
            .Include(r => r.Product)
            .Where(r =>
                r.ProductId == qry.ProductId &&
                r.Status == ReviewStatus.Approved);

        var total = await q.CountAsync(ct);

        q = (qry.SortBy.ToLower(), qry.SortOrder.ToLower()) switch
        {
            ("rating", "asc") => q.OrderBy(r => r.Rating),
            ("rating", _) => q.OrderByDescending(r => r.Rating),
            ("date", "asc") => q.OrderBy(r => r.CreatedAt),
            _ => q.OrderByDescending(r => r.CreatedAt)
        };

        var reviews = await q
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .ToListAsync(ct);

        var userIds = reviews.Select(r => r.UserId).Distinct().ToList();
        var users = await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, ct);

        var items = reviews.Select(r => new ReviewDto(
            Id: r.Id,
            ProductId: r.ProductId,
            ProductName: r.Product.Name,
            UserId: r.UserId,
            UserDisplayName: users.TryGetValue(r.UserId, out var user)
                ? GetDisplayName(user.FirstName, user.LastName, user.UserName)
                : "Anonymous",
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
            ? userName ?? "Anonymous"
            : fullName;
    }
}
