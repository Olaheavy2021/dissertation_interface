namespace Dissertation.Infrastructure.Helpers;

public static class SupervisionCohortRoutes
{
    public const string SupervisionCohortRoute = "/supervisioncohort";

    public const string UnassignedSupervisors = $"{SupervisionCohortRoute}/inactive";

    public const string UpdateSupervisionSlots = $"{SupervisionCohortRoute}/update-slots";

    public const string GetSupervisionCohortMetrics = $"{SupervisionCohortRoute}/metrics/";
}