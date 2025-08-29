using HackerNewsModels;
using HackerNewsModels.Items;

using Refit;

namespace HackerNews.ApiService.Services;

public interface IHackerNewsApi
{
    /// <summary>
    /// Retrieves story by id, or null if not found.
    /// </summary>
    /// <param name="id">Story id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The story, or null if it isn't one.</returns>
    [Get("/v0/item/{id}.json")]
    Task<ApiResponse<Item?>> GetItemAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets up to 500 top stories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Top stories id list</returns>
    [Get("/v0/topstories.json")]
    Task<ApiResponse<long[]>> GetTopItemIdentifierAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets up to 500 newest stories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New stories id list</returns>
    [Get("/v0/newstories.json")]
    Task<ApiResponse<long[]>> GetNewItemIdentifiersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets up to 500 best stories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Best stories id list</returns>
    [Get("/v0/beststories.json")]
    Task<ApiResponse<long[]>> GetBestItemIdentifiersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the maximum ID in use.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The highest ID in use</returns>
    [Get("/v0/maxitem.json")]
    Task<ApiResponse<long>> GetMaxItemIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the latest updates (new stories, comments, etc).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updates ids and user names.</returns>
    [Get("/v0/updates.json")]
    Task<ApiResponse<UpdatedItems>> GetUpdatesAsync(CancellationToken cancellationToken = default);
}
