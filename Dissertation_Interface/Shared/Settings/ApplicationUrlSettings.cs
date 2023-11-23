namespace Shared.Settings;

public class ApplicationUrlSettings
{
    public const string SectionName = "ApplicationUrlSettings";
    public string WebClientUrl { get; set; } = string.Empty;

    public string WebConfirmEmailRoute { get; set; } = string.Empty;

    public string WebResetPasswordRoute { get; set; } = string.Empty;

    public string SupervisorConfirmInviteRoute { get; set; } = string.Empty;

    public string StudentConfirmInviteRoute { get; set; } = string.Empty;
}