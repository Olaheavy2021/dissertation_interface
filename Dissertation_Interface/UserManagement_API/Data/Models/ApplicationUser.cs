using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace UserManagement_API.Data.Models;

[ExcludeFromCodeCoverage]
public class ApplicationUser : IdentityUser
{
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public bool IsLockedOutByAdmin { get; set; }
    public DateTime CreatedOn { get; set; }
    public long? DepartmentId { get; set; }
    public long? CourseId { get; set; }
    public virtual ProfilePicture? ProfilePicture { get; set; } = null!;
    public ICollection<SupervisionCohort> SupervisedCohorts { get; set; } = null!;
}