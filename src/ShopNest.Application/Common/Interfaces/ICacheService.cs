namespace ShopNest.Application.Common.Interfaces;

/// <summary>
/// Distributed cache abstraction.
/// Phases 1-7 used IMemoryCache stubs directly in query handlers.
/// Phase 8 replaces all of them via the CachingBehavior MediatR pipeline.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    Task SetAsync<T>(
        string key, T value, TimeSpan ttl,
        CancellationToken ct = default);

    Task RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Removes all keys beginning with <paramref name="prefix"/> using SCAN
    /// (non-blocking — safe for production Redis).
    /// Call after any mutation that invalidates a group of cached queries.
    /// Example: after CreateProduct → RemoveByPrefixAsync("products:")
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
}