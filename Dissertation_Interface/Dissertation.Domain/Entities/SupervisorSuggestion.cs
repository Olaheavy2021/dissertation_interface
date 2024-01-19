using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class SupervisorSuggestion : AuditableEntity<long>
{
    public int AvailableSlot { get; set; }

    public double CompatibilityScore { get; set; }

    public string ResearchArea { get; set; }

    public string SupervisorId { get; set; }

    public string StudentTopic { get; set; }

    public long StudentId { get; set; }

    private SupervisorSuggestion(
        int availableSlot,
        double compatibilityScore,
        string researchArea,
        string supervisorId,
        string studentTopic,
        long studentId
    )
    {
        AvailableSlot = availableSlot;
        CompatibilityScore = compatibilityScore;
        ResearchArea = researchArea;
        SupervisorId = supervisorId;
        StudentTopic = studentTopic;
        StudentId = studentId;
    }

    public static SupervisorSuggestion Create(
        int availableSlot,
        double compatibilityScore,
        string researchArea,
        string supervisorId,
        string studentTopic,
        long studentId
    ) =>
        new(availableSlot, compatibilityScore, researchArea, supervisorId, studentTopic, studentId);
}