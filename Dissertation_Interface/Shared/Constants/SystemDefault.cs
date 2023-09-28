namespace Shared.Constants;

public static class SystemDefault
{
    public const string DefaultPassword = "SheffieldUK123!";
    public const string DefaultSuperAdmin1 = "superadmin1";
    public const string DefaultSuperAdmin2 = "superadmin2";

    private static readonly DateTime LocalTime = new DateTime(2099, 06, 06, 06, 32, 00);
    public static readonly DateTimeOffset LockOutEndDate = new(LocalTime, TimeZoneInfo.Local.GetUtcOffset(LocalTime));
}