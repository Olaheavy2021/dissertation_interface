namespace Shared.DTO;

public class InitiateMatchingRequest
{
    public List<StudentMatchingRequest>  Student { get; set; } = null!;

    public List<SupervisorMatchingRequest>  Supervisor { get; set; } = null!;

}

public class StudentMatchingRequest
{
    public string StudentTopic { get; set; } = string.Empty;

    public long Id { get; set; }
}

public class SupervisorMatchingRequest
{
    public string ResearchArea { get; set; } = string.Empty;

    public int AvailableSlot { get; set; }

    public string Id { get; set; } = string.Empty;
}