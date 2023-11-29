using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class GetSupervisor
{
    public GetDepartment Department { get; set; }

    public string ProfilePicture { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public string ResearchArea { get; set; } = default!;

    public UserDto User { get; set; }
}