namespace Shared.DTO;

public class SupervisionListDto
{
    public StudentListDto StudentDetails { get; set; } = null!;

    public SupervisorListDto SupervisorDetails { get; set; } = null!;

    public long DissertationCohortId { get; set; }

    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }
}