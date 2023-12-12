using Shared.Helpers;

namespace Shared.DTO;

public class PaginatedSupervisionListDto
{
    public PagedList<SupervisionListDto> Data { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public bool HasNext { get; set; }

    public bool HasPrevious { get; set; }
}