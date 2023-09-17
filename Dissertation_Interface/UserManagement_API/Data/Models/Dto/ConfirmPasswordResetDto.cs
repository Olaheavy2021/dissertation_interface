using Destructurama.Attributed;

namespace UserManagement_API.Data.Models.Dto;

public class ConfirmPasswordResetDto
{
    [NotLogged]
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}