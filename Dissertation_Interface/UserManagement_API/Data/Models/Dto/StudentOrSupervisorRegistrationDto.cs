using Destructurama.Attributed;

namespace UserManagement_API.Data.Models.Dto;

public class StudentOrSupervisorRegistrationDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [NotLogged]
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}