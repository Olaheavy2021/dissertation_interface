using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Helpers;

[ExcludeFromCodeCoverage]
public class DepartmentRoutes
{
    public const string DepartmentRoute = "/department";

    public const string GetAllDepartmentsRoute = $"{DepartmentRoute}/all-departments";
}