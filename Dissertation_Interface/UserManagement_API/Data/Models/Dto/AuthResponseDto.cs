using System.Diagnostics.CodeAnalysis;
using Shared.DTO;

namespace UserManagement_API.Data.Models.Dto;
[ExcludeFromCodeCoverage]
public class AuthResponseDto
{
    public UserDto? User { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public IList<string> Role { get; set; } = default!;
}