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
    /// <returns>Paged response of stories by category.</returns>
    public Task<PagedResponse<Story>> GetStoriesAsync(StoryCategory category, int page, int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to find the first 10 stories with matching text in the title. Case insensitive.
    /// </summary>
    /// <param name="category">Story category (newest, best, top)</param>
    /// <param name="searchText">Search string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any stories where the title contains the searchText</returns>
    public Task<IEnumerable<Story>> FindStory(StoryCategory category, string searchText,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes items with the given ids from the cache.
    /// </summary>
    /// <param name="ids">Story identifiers to remove</param>
    /// <param name="cancellationToken">Cancellation</param>
    /// <returns>Async task</returns>
    public Task RemoveItemsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}
