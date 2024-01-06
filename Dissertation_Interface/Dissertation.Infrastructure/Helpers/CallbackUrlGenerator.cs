namespace Dissertation.Infrastructure.Helpers;

public static class CallbackUrlGenerator
{
    public static string GenerateSupervisionInviteCallBackUrl(string webClientUrl, string route, string staffId,
        string invitationCode) =>
        $"{webClientUrl}/{route}?username={staffId}&code={invitationCode}";

    public static string GenerateStudentInviteCallBackUrl(string webClientUrl, string route, string studentId,
        string invitationCode) =>
        $"{webClientUrl}/{route}?username={studentId}&code={invitationCode}";
}