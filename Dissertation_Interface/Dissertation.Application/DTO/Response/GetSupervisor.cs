using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class GetSupervisor
{
    public GetUserDto? UserDetails { get; set; }

    public SupervisorDto? SupervisorDetails { get; set; }
}