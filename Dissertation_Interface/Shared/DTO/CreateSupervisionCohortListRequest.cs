namespace Shared.DTO;

public class CreateSupervisionCohortListRequest
{
    public List<CreateSupervisionCohortRequest>? SupervisionCohortRequests { get; set; }

    public long DissertationCohortId { get; set; }
}