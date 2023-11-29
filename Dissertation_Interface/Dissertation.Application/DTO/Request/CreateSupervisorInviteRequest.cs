using System.ComponentModel.DataAnnotations;

namespace Dissertation.Application.DTO.Request;

public class CreateSupervisorInviteRequest
{
    [Required]
    public string LastName { get; set; } = default!;
    [Required]
    public string FirstName { get; set; } = default!;
    [Required]
    public string StaffId { get; set; } = default!;
    [Required]
    public string Email { get; set; } = default!;
    [Required]
    public long DepartmentId { get; set; } = default!;
}