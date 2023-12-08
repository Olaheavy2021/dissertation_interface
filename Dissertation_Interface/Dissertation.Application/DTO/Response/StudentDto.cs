using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class StudentDto
{
    public long Id { get; set; }

    public GetCourse Course { get; set; }

    public string UserId { get; set; } = default!;

    public string? ResearchTopic { get; set; }

    public string? ResearchProposal { get; set; }

    public GetDissertationCohort DissertationCohort { get; set; }
}