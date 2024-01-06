using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagement_API.Data.DomainHelper;

namespace UserManagement_API.Data.Models;

public class SupervisionCohort : AuditableEntity<long>
{
    public string SupervisorId { get; set; } = default!;

    [ForeignKey("SupervisorId")]
    public ApplicationUser Supervisor { get; set; }

    public long DissertationCohortId { get; set; }

    public int SupervisionSlot { get; set; }

    public int AvailableSupervisionSlot { get; set; }

    private SupervisionCohort(
        string supervisorId,
        int supervisionSlot,
        long dissertationCohortId
    )
    {
        SupervisorId = supervisorId;
        SupervisionSlot = supervisionSlot;
        DissertationCohortId = dissertationCohortId;
        AvailableSupervisionSlot = supervisionSlot;
    }

    public static SupervisionCohort Create(string supervisorId,
        int supervisionSlot,
        long dissertationCohortId) =>
        new(supervisorId, supervisionSlot, dissertationCohortId);
}