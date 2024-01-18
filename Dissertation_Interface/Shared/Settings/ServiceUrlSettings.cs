namespace Shared.Settings;

public class ServiceUrlSettings
{
    public const string SectionName = "ServiceUrls";

    public string UserApi { get; set; } = default!;

    public string DissertationApi { get; set; } = default!;

    public string DissertationMatchingApi { get; set; } = default!;
}