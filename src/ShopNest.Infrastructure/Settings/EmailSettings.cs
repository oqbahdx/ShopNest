namespace ShopNest.Infrastructure.Settings;

/// <summary>Bound from appsettings.json → "EmailSettings".</summary>
public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string SendGridApiKey  { get; set; } = string.Empty;
    public string FromEmail       { get; set; } = string.Empty;
    public string FromName        { get; set; } = "ShopNest";
    public string FrontendBaseUrl { get; set; } = "https://app.shopnest.com";
}
