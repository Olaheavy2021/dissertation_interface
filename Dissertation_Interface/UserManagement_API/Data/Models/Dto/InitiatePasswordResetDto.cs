using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class InitiatePasswordResetDto
{
    public string Email { get; set; } = string.Empty;
}