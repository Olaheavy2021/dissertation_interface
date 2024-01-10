using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class EditUserRequestDto
{
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}