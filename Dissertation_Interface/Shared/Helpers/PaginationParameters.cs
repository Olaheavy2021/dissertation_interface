namespace Shared.Helpers;

public class PaginationParameters
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    public string? LoggedInAdminId { get; set; } = string.Empty;

    public string SearchQuery { get; set; } = string.Empty;

    public string FilterBy { get; set; } = string.Empty;

    private int _pageSize = 10;
    public int PageSize
    {
        get => this._pageSize;
        set => this._pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}