using System.ComponentModel.DataAnnotations.Schema;
using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class Supervisor : AuditableEntity<long>
{
    public long DepartmentId { get; set; }

    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }

    public string? ProfilePicture { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string? ResearchArea { get; set; } = default!;

    private Supervisor(
        string userId,
        long departmentId
        )
    {
        DepartmentId = departmentId;
        UserId = userId;
    }

    public static Supervisor Create(
        string userId,
        long departmentId
    ) =>
        new(userId, departmentId);
}