namespace Shared.Settings;

public class ServiceBusSettings
{
    public const string SectionName = "ServiceBusSettings";
    public string ServiceBusConnectionString { get; set; } = string.Empty;

    public string AuditLoggerQueue { get; set; } = string.Empty;

    public string EmailLoggerQueue { get; set; } = string.Empty;
}