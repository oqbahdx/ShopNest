using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Coupons.Queries.GetCoupons;

public sealed class GetCouponsQueryHandler
    : IRequestHandler<GetCouponsQuery, Result<PagedResult<CouponDto>>>
{
    private readonly IAppDbContext _db;

    public GetCouponsQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PagedResult<CouponDto>>> Handle(
        GetCouponsQuery qry, CancellationToken ct)
    {
        var q = _db.Coupons.AsNoTracking().AsQueryable();

        if (qry.IsActive.HasValue)
            q = q.Where(c => c.IsActive == qry.IsActive.Value);

        var total = await q.CountAsync(ct);

        q = (qry.SortBy.ToLower(), qry.SortOrder.ToLower()) switch
        {
            ("code", "asc") => q.OrderBy(c => c.Code),
            ("code", _) => q.OrderByDescending(c => c.Code),
            ("usagecount", "asc") => q.OrderBy(c => c.UsageCount),
            ("usagecount", _) => q.OrderByDescending(c => c.UsageCount),
            ("expiresat", "asc") => q.OrderBy(c => c.ExpiresAt),
            ("expiresat", _) => q.OrderByDescending(c => c.ExpiresAt),
            _ => q.OrderByDescending(c => c.CreatedAt)
        };

        var coupons = await q
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .ToListAsync(ct);

        var items = coupons.Select(c => new CouponDto(
            Id: c.Id,
            Code: c.Code,
            DiscountType: c.DiscountType.ToString(),
            DiscountValue: c.DiscountValue,
            MinimumOrderAmount: c.MinimumOrderAmount,
            MaximumDiscountAmount: c.MaximumDiscountAmount,
            UsageLimit: c.UsageLimit,
            UsageCount: c.UsageCount,
            ExpiresAt: c.ExpiresAt,
            IsActive: c.IsActive
        )).ToList();

        return Result<PagedResult<CouponDto>>.Success(
            PagedResult<CouponDto>.Create(
                items, qry.Page, qry.PageSize, total));
    }
}