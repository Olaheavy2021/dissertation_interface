using System.ComponentModel.DataAnnotations;

namespace Dissertation.Infrastructure.DTO;

public class UserUploadRequest
{
    [Required]
    public string LastName { get; set; } = default!;
    [Required]
    public string FirstName { get; set; } = default!;
    [Required]
    public string Username { get; set; } = default!;
    [Required]
    public string Email { get; set; } = default!;
}