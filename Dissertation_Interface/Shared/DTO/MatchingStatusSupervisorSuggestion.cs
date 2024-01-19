namespace Shared.DTO;

public class MatchingStatusSupervisorSuggestion
{
    public int AvailableSlot { get; set; }
    public double CompatibilityScore { get; set; }
    public string ResearchArea { get; set; } = string.Empty;
    public string SupervisorId { get; set; } = string.Empty;
}