namespace Dissertation.Infrastructure.Helpers;

public static class UserApiUrlRoutes
{
    public const string UserRoute = "/user/";

    public const string GetUserByEmailRoute = $"{UserRoute}get-by-email/";

    public const string GetUserByUserNameRoute = $"{UserRoute}get-by-username/";

    public const string RegisterSupervisorRoute = $"{UserRoute}register-supervisor";

    public const string RegisterStudentRoute = $"{UserRoute}register-student";

    public const string GetSupervisors = $"{UserRoute}get-supervisors";

    public const string GetStudents = $"{UserRoute}get-students";

    public const string EditStudentRoute = $"{UserRoute}edit-student";

    public const string EditSupervisorRoute = $"{UserRoute}edit-supervisor";

    public const string AssignSupervisorRoleToAdmin = $"{UserRoute}assign-supervisor-role";

    public const string AssignAdminRoleToSupervisor = $"{UserRoute}assign-admin-role";

}