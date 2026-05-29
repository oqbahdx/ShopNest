namespace ShopNest.Infrastructure.Settings;

/// <summary>
/// Bound from appsettings.json section "Stripe".
/// Never commit real keys — use User Secrets or environment variables.
/// </summary>
public sealed class StripeSettings
{
    public const string SectionName = "Stripe";

    public string PublishableKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string WebhookSecret { get; init; } = string.Empty;
}