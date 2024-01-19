namespace Dissertation.Application.DTO.Response;

public class GetSupervisorSuggestion
{
    public double CompatibilityScore { get; set; }

    public GetSupervisor Supervisor { get; set; } = null!;
}