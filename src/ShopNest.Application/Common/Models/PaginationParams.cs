namespace ShopNest.Application.Common.Models;

public sealed record PaginationParams(
    int Page = 1,
    int PageSize = 20)
{
    public const int MaxPageSize = 100;
    public int Page { get; init; } = Math.Max(1, Page);
    public int PageSize { get; init; } = Math.Clamp(PageSize, 1, MaxPageSize);
    public int Skip => (Page - 1) * PageSize;
}