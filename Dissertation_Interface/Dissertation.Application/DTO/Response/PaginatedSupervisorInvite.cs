using Shared.Helpers;

namespace Dissertation.Application.DTO.Response;

public class PaginatedSupervisorInvite
{
    public PagedList<GetSupervisorInvite> Data { get; set; } = null!;

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public bool HasNext { get; set; }

    public bool HasPrevious { get; set; }
}