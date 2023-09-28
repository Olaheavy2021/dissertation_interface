using Destructurama.Attributed;

namespace UserManagement_API.Data.Models.Dto;

public class LoginRequestDto
{
    public string UserName { get; set; } = string.Empty;
    [NotLogged]
    public string Password { get; set; } = string.Empty;
}