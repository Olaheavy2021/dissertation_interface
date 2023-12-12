namespace Dissertation.Infrastructure.Helpers;

public class SupervisionListRoutes
{
    public const string SupervisionListRoute = "/supervisionlist";

    public const string GetSupervisionListsForAStudent = $"{SupervisionListRoute}/student";

    public const string GetSupervisionListsForASupervisor = $"{SupervisionListRoute}/supervisor";
}