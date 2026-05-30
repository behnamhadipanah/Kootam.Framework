using Kootam.Extensions.Cqrs.Abstractions.Queries;

namespace Kootam.Framework.Abstractions;

public interface IPageQuery<T> : IQuery<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int SkipCount => (PageNumber - 1) * PageSize;
    public bool HasTotalCount { get; set; }
    public string SortBy { get; set; }

}


public abstract class PageQuery<T> : IPageQuery<T>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int SkipCount => (PageNumber - 1) * PageSize;
    public bool HasTotalCount { get; set; }
    public string SortBy { get; set; } = "Id";
}

public class PageData<T>
{
    public List<T> QueryResult { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }

}