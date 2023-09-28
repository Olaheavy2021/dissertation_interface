namespace UserManagement_API.Settings;

public class ServiceBusSettings
{
    public string ServiceBusConnectionString { get; set; } = string.Empty;

    public string RegisterAdminUserQueue { get; set; } = string.Empty;

    public string ResetPasswordQueue { get; set; } = string.Empty;

    public string AccountLockedOutQueue { get; set; } = string.Empty;
}