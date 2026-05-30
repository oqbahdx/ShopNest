using MediatR;
using ShopNest.Application.Features.Reviews.DTOs;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Reviews.Queries.GetPendingReviews;

public sealed record GetPendingReviewsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<AdminReviewDto>>>;

public sealed class GetPendingReviewsQueryHandler
    : IRequestHandler<GetPendingReviewsQuery, Result<PagedResult<AdminReviewDto>>>
{
    private readonly IAppDbContext _db;

    public GetPendingReviewsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PagedResult<AdminReviewDto>>> Handle(
        GetPendingReviewsQuery qry, CancellationToken ct)
    {
        var q = _db.Reviews
            .AsNoTracking()
            .Include(r => r.Product)
            .Where(r => r.Status == ReviewStatus.Pending)
            .OrderBy(r => r.CreatedAt); // oldest first — FIFO moderation queue

        var total = await q.CountAsync(ct);

        var reviews = await q
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .ToListAsync(ct);

        var userIds = reviews.Select(r => r.UserId).Distinct().ToList();
        var users = await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, ct);

        var items = reviews.Select(r => new AdminReviewDto(
            Id: r.Id,
            ProductId: r.ProductId,
            ProductName: r.Product.Name,
            UserId: r.UserId,
            UserEmail: users.TryGetValue(r.UserId, out var user)
                ? user.Email ?? string.Empty
                : string.Empty,
            Rating: r.Rating,
            Title: r.Title ?? string.Empty,
            Comment: r.Comment ?? string.Empty,
            IsVerifiedPurchase: r.IsVerifiedPurchase,
            Status: r.Status,
            RejectionNote: r.AdminNote,
            CreatedAt: r.CreatedAt
        )).ToList();

        return Result<PagedResult<AdminReviewDto>>.Success(
            PagedResult<AdminReviewDto>.Create(
                items, qry.Page, qry.PageSize, total));
    }
}
