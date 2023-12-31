namespace Shared.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Secret { get; set; } = string.Empty;

    public double DurationInMinutes { get; set; }

    public int RefreshTokenValidityInDays { get; set; }
}