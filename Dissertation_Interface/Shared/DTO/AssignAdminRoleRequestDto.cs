namespace UserManagement_API.Data.Models.Dto;

public class AssignAdminRoleRequestDto
{
    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;
}