namespace Shared.Constants;

public static class ServiceBusQueues
{
    public const string RegisterAdminUserQueue = "emailregisteradminuser";
    public const string ResetPasswordQueue = "emailresetpassword";
    public const string AccountLockedOutQueue = "emailaccountlockedout";
    public const string AuditLoggerQueue = "auditlogger";
}