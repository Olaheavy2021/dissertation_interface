using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class StudentDto
{
    public long Id { get; set; }

    public GetCourse Course { get; set; } = null!;

    public string UserId { get; set; } = default!;

    public string? ResearchTopic { get; set; }

    public GetDissertationCohort DissertationCohort { get; set; } = null!;

    public GetResearchProposal ResearchProposal { get; set; } = null!;
}