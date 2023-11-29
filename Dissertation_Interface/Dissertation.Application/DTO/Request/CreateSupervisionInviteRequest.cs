namespace Dissertation.Application.DTO.Request;

public class CreateSupervisionInviteRequest
{
    public string LastName { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string StaffId { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Department { get; set; } = default!;

    public int StudentAllocation { get; set; }
}