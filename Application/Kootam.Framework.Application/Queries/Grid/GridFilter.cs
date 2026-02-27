namespace Kootam.Framework.Application.Queries.Grid;

public readonly record struct GridFilter(
    int Skip,
    int PageSize,
    string? SortColumn,
    SortDirection SortDirection,
    IReadOnlyDictionary<string, string>? Filters
);


public enum SortDirection
{
    Asc = 0,
    Desc = 1
}