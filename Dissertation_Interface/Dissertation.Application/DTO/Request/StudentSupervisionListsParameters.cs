using Shared.Enums;
using Shared.Helpers;

namespace Dissertation.Application.DTO.Request;

public class StudentSupervisionListsParameters : PaginationParameters
{
    public string SearchBySupervisor { get; set; } = string.Empty;
}