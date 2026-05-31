namespace ShopNest.Infrastructure.Settings;

public sealed class RedisSettings
{
    public const string SectionName = "Redis";

    /// <summary>
    /// Connection string, e.g. "localhost:6379"
    /// or full URL for cloud Redis (Upstash, Redis Cloud).
    /// </summary>
    public string ConnectionString { get; init; } = "localhost:6379";
}