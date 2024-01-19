using System.Diagnostics.CodeAnalysis;
using SendGrid;

namespace Notification_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class EmailResponseDto
{
    public Response? Result { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string? Message { get; set; } = string.Empty;
}