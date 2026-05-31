namespace ShopNest.Application.Common.Cache;

/// <summary>
/// Single source of truth for all Redis cache key strings.
/// Prefix-based grouping enables efficient RemoveByPrefixAsync invalidation.
/// </summary>
public static class CacheKeys
{
    public static class Products
    {
        public const string Prefix   = "products:";
        public const string Featured = "products:featured";
        public static string ById(Guid id)       => $"products:{id}";
        public static string BySlug(string slug) => $"products:slug:{slug}";
    }

    public static class Categories
    {
        public const string Prefix = "categories:";
        public const string All    = "categories:all";
        public static string BySlug(string slug) => $"categories:slug:{slug}";
    }

    public static class Dashboard
    {
        public const string Prefix  = "admin:dashboard:";
        public const string Summary = "admin:dashboard:summary";
    }

    public static class Admin
    {
        public static string TopProducts(int top, string by)
            => $"admin:top-products:{top}:{by}";

        public static string UserGrowth(string from, string to, string g)
            => $"admin:user-growth:{from}:{to}:{g}";

        public static string LowStock(int page, int size)
            => $"admin:low-stock:{page}:{size}";

        public static string Revenue(string from, string to, int page)
            => $"admin:revenue:{from}:{to}:{page}";
    }
}