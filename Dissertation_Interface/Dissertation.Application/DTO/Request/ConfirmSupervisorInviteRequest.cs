namespace Dissertation.Application.DTO.Request;

public class ConfirmSupervisorInviteRequest
{
    public string StaffId { get; set; } = default!;

    public string InvitationCode { get; set; } = default!;
}