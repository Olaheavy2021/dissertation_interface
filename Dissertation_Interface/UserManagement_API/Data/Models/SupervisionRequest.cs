using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums;
using UserManagement_API.Data.DomainHelper;

namespace UserManagement_API.Data.Models;

public class SupervisionRequest : AuditableEntity<long>
{
    public string SupervisorId { get; set; } = default!;

    [ForeignKey("SupervisorId")]
    public ApplicationUser Supervisor { get; set; }

    public string StudentId { get; set; } = default!;

    [ForeignKey("StudentId")]
    public ApplicationUser Student { get; set; }

    public long DissertationCohortId { get; set; }

    public SupervisionRequestStatus Status { get; set; }

    public string? Comment { get; set; }

    private SupervisionRequest(
        string supervisorId,
        string studentId,
        long dissertationCohortId
    )
    {
        SupervisorId = supervisorId;
        StudentId = studentId;
        DissertationCohortId = dissertationCohortId;
        Status = SupervisionRequestStatus.Pending;
    }

    public static SupervisionRequest Create(
        string supervisorId,
        string studentId,
        long dissertationCohortId) =>
        new(supervisorId, studentId, dissertationCohortId);
}