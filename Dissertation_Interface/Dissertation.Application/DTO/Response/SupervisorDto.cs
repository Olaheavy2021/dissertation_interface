using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class SupervisorDto
{
    public long Id { get; set; }

    public GetDepartment Department { get; set; }

    public string UserId { get; set; } = default!;

    public string? ResearchArea { get; set; }
}