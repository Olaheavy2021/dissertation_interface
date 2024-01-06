using Shared.Enums;

namespace Shared.DTO;

public class SupervisionRequestListDto
{
    public long Id { get; set; }
    public StudentListDto StudentDetails { get; set; } = null!;

    public SupervisorListDto SupervisorDetails { get; set; } = null!;

    public long DissertationCohortId { get; set; }

    public SupervisionRequestStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}