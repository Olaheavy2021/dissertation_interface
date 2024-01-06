namespace Shared.DTO;

public class AssignSupervisorRoleRequestDto
{
    public string Email { get; set; } = default!;

    public long DepartmentId { get; set; }
}