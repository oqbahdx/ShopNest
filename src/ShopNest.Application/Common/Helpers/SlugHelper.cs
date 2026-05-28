using System.Text.RegularExpressions;

namespace ShopNest.Application.Common.Helpers;

public static partial class SlugHelper
{
    public static string FromText(string value)
    {
        var slug = value.Trim().ToLowerInvariant();
        slug = NonAlphaNumeric().Replace(slug, "-");
        slug = DuplicateDashes().Replace(slug, "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("N") : slug;
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphaNumeric();

    [GeneratedRegex("-+")]
    private static partial Regex DuplicateDashes();
}
