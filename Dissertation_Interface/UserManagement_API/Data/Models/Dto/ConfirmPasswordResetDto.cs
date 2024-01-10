using System.Diagnostics.CodeAnalysis;
using Destructurama.Attributed;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class ConfirmPasswordResetDto
{
    [NotLogged]
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}