namespace ShopNest.Application.Common.Interfaces;

/// <summary>
/// Marker interface for MediatR queries whose results should be cached.
/// CachingBehavior intercepts any IRequest that implements this interface
/// — no cache code needed in the handler itself.
///
/// Example:
///   public sealed record GetCategoriesQuery
///       : IRequest&lt;Result&lt;List&lt;CategoryDto&gt;&gt;&gt;, ICacheableQuery
///   {
///       public string   CacheKey => CacheKeys.Categories.All;
///       public TimeSpan Ttl      => TimeSpan.FromMinutes(60);
///   }
/// </summary>
public interface ICacheableQuery
{
    string   CacheKey { get; }
    TimeSpan Ttl      { get; }
}