using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UserManagement_API.Data.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }

    public bool IsLockedOutByAdmin { get; set; }
}