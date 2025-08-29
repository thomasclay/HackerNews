using HackerNewsModels.Items;

namespace HackerNewsModels;

/// <summary>
/// For retrieving hacker news information
/// </summary>
public interface IHackerNewsService
{
    /// <summary>
    /// Attempts to get the item with the given id
    /// </summary>
    /// <param name="id">Story identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The story, or null</returns>
    public Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the stories, paged, based on category.
    /// </summary>
    /// <param name="category">Story category (newest, best, top)</param>
    /// <param name="page">Page number (0 indexed)</param>
    /// <param name="pageSize">Page size, 1-100</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged response of newest stories.</returns>
    public Task<PagedResponse<Story>> GetStoriesAsync(StoryCategory category, int page, int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to find the first 10 items with 
    /// matching text in the title.
    /// </summary>
    /// <param name="category">Story category (newest, best, top)</param>
    /// <param name="searchText">Search string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any stories where the title matches</returns>
    public Task<IEnumerable<Story>> FindAsync(StoryCategory category, string searchText,
        CancellationToken cancellationToken = default);
}
