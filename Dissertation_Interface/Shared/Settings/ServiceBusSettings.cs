namespace Shared.Settings;

public class ServiceBusSettings
{
    public string ServiceBusConnectionString { get; set; } = string.Empty;

    public string RegisterAdminUserQueue { get; set; } = string.Empty;

    public string ResetPasswordQueue { get; set; } = string.Empty;

    public string AccountLockedOutQueue { get; set; } = string.Empty;

    public string AuditLoggerQueue { get; set; } = string.Empty;

    public string EmailLoggerQueue { get; set; } = string.Empty;
}