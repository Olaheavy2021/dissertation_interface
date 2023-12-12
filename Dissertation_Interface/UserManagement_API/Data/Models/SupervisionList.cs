using System.ComponentModel.DataAnnotations.Schema;
using UserManagement_API.Data.DomainHelper;

namespace UserManagement_API.Data.Models;

public class SupervisionList : AuditableEntity<long>
{
    public string SupervisorId { get; set; } = default!;

    [ForeignKey("SupervisorId")]
    public ApplicationUser Supervisor { get; set; }

    public string StudentId { get; set; } = default!;

    [ForeignKey("StudentId")]
    public ApplicationUser Student { get; set; }

    public long DissertationCohortId { get; set; }

    private SupervisionList(
        string supervisorId,
        string studentId,
        long dissertationCohortId
    )
    {
        SupervisorId = supervisorId;
        StudentId = studentId;
        DissertationCohortId = dissertationCohortId;
    }

    public static SupervisionList Create(
        string supervisorId,
        string studentId,
        long dissertationCohortId) =>
        new(supervisorId, studentId, dissertationCohortId);
}