namespace Shared.Constants;

public class Roles
{
    public const string RoleSuperAdmin = "superadmin";
    public const string RoleAdmin = "admin";
    public const string RoleStudent = "student";
    public const string RoleSupervisor = "supervisor";

    public static readonly string[] ApplicationRoles = { RoleSuperAdmin, RoleAdmin, RoleStudent, RoleSupervisor };
}