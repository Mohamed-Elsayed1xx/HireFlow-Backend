namespace HireFlow.Application.Common.Models;

public class PaginationParams
{
    private const int MaxPageSize = 50;
    private int _pageSize = 20;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}