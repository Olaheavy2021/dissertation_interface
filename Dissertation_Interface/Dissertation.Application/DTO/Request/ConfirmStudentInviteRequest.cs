namespace Dissertation.Application.DTO.Request;

public class ConfirmStudentInviteRequest
{
    public string StudentId { get; set; } = default!;

    public string InvitationCode { get; set; } = default!;
}