namespace HireFlow.Application.Common.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}