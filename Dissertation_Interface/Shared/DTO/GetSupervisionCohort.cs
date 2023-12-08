namespace Shared.DTO;

public class GetSupervisionCohort
{
    public long Id { get; set; }
    public UserDto? UserDetails  { get; set; }

    public long DissertationCohortId { get; set; }

    public int SupervisionSlot { get; set; }
}