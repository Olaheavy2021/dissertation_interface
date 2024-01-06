using Shared.Enums;
using Shared.Helpers;

namespace Dissertation.Application.DTO.Request;

public class SupervisorSupervisionListParameters : PaginationParameters
{
    public SupervisionRequestStatus FilterByStatus { get; set; }

    public string SearchByStudent { get; set; } = string.Empty;

    public long FilterByCohort { get; set; }
}