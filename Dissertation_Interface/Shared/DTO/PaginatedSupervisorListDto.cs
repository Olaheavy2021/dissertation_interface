using Shared.Helpers;

namespace Shared.DTO;

public class PaginatedSupervisorListDto
{
    public PagedList<SupervisorListDto> Data { get; set; } = null!;

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public bool HasNext { get; set; }

    public bool HasPrevious { get; set; }

}