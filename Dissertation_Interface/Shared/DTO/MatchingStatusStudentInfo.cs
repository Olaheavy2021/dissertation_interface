namespace Shared.DTO;

public class MatchingStatusStudentInfo
{
    public string StudentTopic { get; set; } = string.Empty;
    public List<MatchingStatusSupervisorSuggestion> SupervisorSuggestions { get; set; } = null!;
}