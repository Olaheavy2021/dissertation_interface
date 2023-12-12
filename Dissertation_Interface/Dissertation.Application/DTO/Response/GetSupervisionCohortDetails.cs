using Shared.DTO;

namespace Dissertation.Application.DTO.Response;

public class GetSupervisionCohortDetails
{
    public long Id { get; set; }
    public UserDto? UserDetails  { get; set; }

    public long DissertationCohortId { get; set; }

    public int SupervisionSlot { get; set; }

    public int AvailableSupervisionSlot { get; set; }

    public SupervisorDto? SupervisorDetails { get; set; }
}