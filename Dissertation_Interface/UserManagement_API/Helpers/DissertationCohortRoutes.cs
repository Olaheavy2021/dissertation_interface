using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Helpers;

[ExcludeFromCodeCoverage]
public class DissertationCohortRoutes
{
    public const string DissertationCohortRoute = "/dissertationcohort";

    public const string GetActiveDissertationCohortRoute = $"{DissertationCohortRoute}/active";
}