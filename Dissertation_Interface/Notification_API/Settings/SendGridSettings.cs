namespace Notification_API.Settings;

public class SendGridSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TestEmail { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
}