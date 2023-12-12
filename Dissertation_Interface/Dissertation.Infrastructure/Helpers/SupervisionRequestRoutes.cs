namespace Dissertation.Infrastructure.Helpers;

public static class SupervisionRequestRoutes
{
    public const string SupervisionRequestRoute = "/supervisionrequest";

    public const string GetSupervisionRequestsForAStudent = $"{SupervisionRequestRoute}/student";

    public const string GetSupervisionRequestsForASupervisor = $"{SupervisionRequestRoute}/supervisor";

    public const string RejectSupervisionRequest = $"{GetSupervisionRequestsForASupervisor}/reject";

    public const string AcceptSupervisionRequest = $"{GetSupervisionRequestsForASupervisor}/accept";

    public const string CancelSupervisionRequest = $"{GetSupervisionRequestsForAStudent}/cancel";
}