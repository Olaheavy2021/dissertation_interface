using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class EmailRequestDto
{
    public string Email { get; set; } = string.Empty;
}