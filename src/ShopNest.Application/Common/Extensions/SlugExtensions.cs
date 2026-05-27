using System.Text.RegularExpressions;

namespace ShopNest.Application.Common.Extensions;

public static class SlugExtensions
{
    private static readonly Regex NonAlphanumericRegex =
        new(@"[^a-z0-9\s-]", RegexOptions.Compiled);

    private static readonly Regex MultipleSpacesRegex =
        new(@"[\s-]+", RegexOptions.Compiled);

    public static string ToSlug(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        var slug = value.ToLowerInvariant().Trim();
        slug = NonAlphanumericRegex.Replace(slug, string.Empty);
        slug = MultipleSpacesRegex.Replace(slug, "-");
        return slug.Trim('-');
    }
}