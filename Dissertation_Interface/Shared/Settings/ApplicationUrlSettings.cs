namespace Shared.Settings;

public class ApplicationUrlSettings
{
    public string WebClientUrl { get; set; } = string.Empty;

    public string WebConfirmEmailRoute { get; set; } = string.Empty;

    public string WebResetPasswordRoute { get; set; } = string.Empty;
}