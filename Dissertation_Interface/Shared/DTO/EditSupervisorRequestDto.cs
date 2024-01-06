namespace Shared.DTO;

public class EditSupervisorRequestDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string StaffId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public long DepartmentId { get; set; }
}