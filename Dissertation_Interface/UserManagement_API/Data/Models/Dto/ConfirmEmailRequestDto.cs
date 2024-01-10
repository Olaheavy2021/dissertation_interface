using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class ConfirmEmailRequestDto
{
    public string UserName { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}