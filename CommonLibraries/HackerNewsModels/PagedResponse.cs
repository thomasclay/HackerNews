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
    public int Page { get; init => field = Math.Max(0, value); } = 0;

    /// <summary>
    /// Requested page size - ranges from 5 to 50.
    /// </summary>
    public required int Size { get; init => field = Math.Clamp(value, 5, 50); }

    /// <summary>
    /// Returned items.
    /// </summary>
    public required IEnumerable<T> Items { get; init; }
}
