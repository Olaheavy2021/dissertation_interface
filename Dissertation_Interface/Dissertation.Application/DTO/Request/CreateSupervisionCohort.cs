using Shared.DTO;

namespace Dissertation.Application.DTO.Request;

public class CreateSupervisionCohort
{
    public List<CreateSupervisionCohortRequest>? SupervisionCohortRequests { get; set; }
}