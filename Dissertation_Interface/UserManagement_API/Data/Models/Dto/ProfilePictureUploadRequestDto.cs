using System.Diagnostics.CodeAnalysis;

namespace UserManagement_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class ProfilePictureUploadRequestDto
{
    public IFormFile? File { get; set; } = null!;

    public string LastName { get; set; } = default!;

    public string FirstName { get; set; } = default!;
}