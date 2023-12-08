namespace Shared.DTO;

public class CreateSupervisionCohortRequest
{
    public string UserId { get; set; } = default!;

    public int SupervisionSlot { get; set; }
}