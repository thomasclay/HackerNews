namespace HackerNewsModels;

/// <summary>
/// Response records of paged data.
/// </summary>
/// <typeparam name="T">Items to retrieve</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// Requested page number (starting with 0)
    /// </summary>
    public required int Page { get; init => field = Math.Max(0, value); }

    /// <summary>
    /// Requested page size - ranges from 5 to 50.
    /// </summary>
    public required int PageSize { get; init => field = Math.Clamp(value, 5, 50); }

    /// <summary>
    /// Returned items.
    /// </summary>
    public required IEnumerable<T> Items { get; init; }
}
