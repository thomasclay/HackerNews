namespace HackerNewsModels;

/// <summary>
/// Response records of paged data.
/// </summary>
/// <typeparam name="T">Items to retrieve</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Total number of items available (not just in this page)
    /// </summary>
    public required long TotalCount { get; init; }

    /// <summary>
    /// Requested page number (starting with 0)
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    /// Requested page size
    /// </summary>
    public required int Size { get; init; }

    /// <summary>
    /// Returned items.
    /// </summary>
    public required IEnumerable<T> Items { get; init; }
}
