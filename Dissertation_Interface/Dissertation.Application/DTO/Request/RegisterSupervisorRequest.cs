namespace Dissertation.Application.DTO.Request;

public class RegisterSupervisorRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string StaffId { get; set; } = default!;
    public long DepartmentId { get; set; }
    public string InvitationCode { get; set; } = default!;
    public string Password { get; set; } = default!;
}