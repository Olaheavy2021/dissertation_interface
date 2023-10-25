namespace Shared.Constants;

public static class EventDescription
{
    public const string ResendEmailConfirmation = "Admin user resends email confirmation to this user - {email}";

    public const string EditUser = "Super Admin user modifies this admin user - {email}";

    public const string LockOutUser = "Admin user deactivates out this user - {email}";

    public const string UnlockUser = "Admin user activates out this user - {email}";

    public const string RegisterAdminUser = "Super Admin user creates another admin user - {email}";
}