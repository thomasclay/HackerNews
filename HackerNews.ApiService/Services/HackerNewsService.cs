using HackerNewsModels;
using HackerNewsModels.Items;

namespace HackerNews.ApiService.Services;

/// <summary>
/// Wrapper to cached articles and stories.
/// </summary>
/// <param name="messageCache">Cache to use</param>
public class HackerNewsService(MessageCacheService messageCache) : IHackerNewsService
{
    private readonly MessageCacheService _messageCache = messageCache;

    /// <inheritdoc />
    public Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken = default)
    {
        return this._messageCache.GetItemAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PagedResponse<Story>> GetStoriesAsync(StoryCategory category, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IEnumerable<Story>> FindStory(StoryCategory category, string searchText, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task RemoveItemsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return this._messageCache.RemoveItemsAsync(ids, cancellationToken);
    }
}
