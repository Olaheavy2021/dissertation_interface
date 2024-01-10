using System.Diagnostics.CodeAnalysis;
using Destructurama.Attributed;

namespace UserManagement_API.Data.Models.Dto;
[ExcludeFromCodeCoverage]
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    [NotLogged]
    public string Password { get; set; } = string.Empty;
}