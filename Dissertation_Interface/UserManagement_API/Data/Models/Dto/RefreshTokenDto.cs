using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class RefreshTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}