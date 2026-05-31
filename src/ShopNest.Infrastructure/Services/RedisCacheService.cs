using System.Text.Json;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Interfaces;
using StackExchange.Redis;

namespace ShopNest.Infrastructure.Services;

/// <summary>
/// Redis implementation of ICacheService using StackExchange.Redis.
/// All methods degrade gracefully — a Redis outage causes cache misses,
/// not application errors.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase                  _db;
    private readonly IConnectionMultiplexer     _redis;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheService(
        IConnectionMultiplexer      redis,
        ILogger<RedisCacheService>  logger)
    {
        _redis  = redis;
        _db     = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key, CancellationToken ct = default)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty
                ? default
                : JsonSerializer.Deserialize<T>((ReadOnlySpan<byte>)value!, JsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GET failed for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key, T value, TimeSpan ttl,
        CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOpts);
            await _db.StringSetAsync(key, json, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SET failed for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(
        string key, CancellationToken ct = default)
    {
        try { await _db.KeyDeleteAsync(key); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis DEL failed for key: {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(
        string prefix, CancellationToken ct = default)
    {
        try
        {
            // SCAN is non-blocking — safe for production Redis
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys   = server.Keys(pattern: $"{prefix}*").ToArray();

            if (keys.Length > 0)
                await _db.KeyDeleteAsync(keys);

            _logger.LogDebug(
                "Invalidated {Count} keys with prefix '{Prefix}'",
                keys.Length, prefix);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex, "Redis prefix delete failed for: {Prefix}", prefix);
        }
    }
}