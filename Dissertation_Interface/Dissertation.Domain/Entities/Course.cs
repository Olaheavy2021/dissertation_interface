using System.ComponentModel.DataAnnotations.Schema;
using Dissertation.Domain.DomainHelper;
using Shared.Enums;

namespace Dissertation.Domain.Entities;

public class Course : AuditableEntity<long>
{
    public long DepartmentId { get; set; }

    public string Name { get; set; } = default!;

    public DissertationConfigStatus Status { get; set; }

    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }

    private Course(
        string name,
        long departmentId
        )
    {
        DepartmentId = departmentId;
        Name = name;
        Status = DissertationConfigStatus.Active;
    }

    public static Course Create(string name, long departmentId) =>
        new(name, departmentId);
}