using MediatR;
using Microsoft.Extensions.Logging;

namespace ShopNest.Application.Common.Behaviors;


/// <summary>
/// MediatR pipeline behavior: transparently caches responses for any query
/// that implements ICacheableQuery.
///
/// Add after ValidationBehavior and LoggingBehavior:
///   cfg.AddBehavior(typeof(IPipelineBehavior&lt;,&gt;), typeof(CachingBehavior&lt;,&gt;));
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(
        ICacheService cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache  = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (request is not ICacheableQuery q)
            return await next();

        // Cache hit
        var cached = await _cache.GetAsync<TResponse>(q.CacheKey, ct);
        if (cached is not null)
        {
            _logger.LogDebug("Cache HIT  {Key}", q.CacheKey);
            return cached;
        }

        // Cache miss — execute handler then store
        _logger.LogDebug("Cache MISS {Key}", q.CacheKey);
        var response = await next();

        await _cache.SetAsync(q.CacheKey, response, q.Ttl, ct);
        return response;
    }
}