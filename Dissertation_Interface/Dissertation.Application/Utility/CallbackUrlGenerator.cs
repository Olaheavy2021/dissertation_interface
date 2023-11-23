namespace Dissertation.Application.Utility;

public static class CallbackUrlGenerator
{
    public static string GenerateSupervisionInviteCallBackUrl(string webClientUrl, string route, string staffId,
        string invitationCode) =>
        $"{webClientUrl}/{route}?username={staffId}&code={invitationCode}";
}