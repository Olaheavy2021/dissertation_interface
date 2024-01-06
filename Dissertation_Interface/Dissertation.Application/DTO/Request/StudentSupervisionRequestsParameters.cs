using Shared.Enums;
using Shared.Helpers;

namespace Dissertation.Application.DTO.Request;

public class StudentSupervisionRequestParameters : PaginationParameters
{
    public SupervisionRequestStatus FilterByStatus { get; set; }

    public string SearchBySupervisor { get; set; } = string.Empty;
}