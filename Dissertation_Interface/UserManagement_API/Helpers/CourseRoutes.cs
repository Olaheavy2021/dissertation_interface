using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Helpers;

[ExcludeFromCodeCoverage]
public class CourseRoutes
{
    public const string CourseRoute = "/course";

    public const string GetAllCoursesRoute = $"{CourseRoute}/all-courses";
}