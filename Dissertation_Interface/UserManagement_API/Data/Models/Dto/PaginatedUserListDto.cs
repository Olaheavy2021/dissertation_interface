using Shared.Helpers;

namespace UserManagement_API.Data.Models.Dto;

public class PaginatedUserListDto
{
    public PagedList<UserListDto> Data { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public bool HasNext { get; set; }

    public bool HasPrevious { get; set; }

}