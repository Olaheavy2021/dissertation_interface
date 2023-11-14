using System.ComponentModel.DataAnnotations.Schema;
using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class Course : AuditableEntity<long>
{
    public long DepartmentId { get; set; }

    public string Name { get; set; } = default!;

    public DissertationConfigStatus Status { get; set; }

    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }
}