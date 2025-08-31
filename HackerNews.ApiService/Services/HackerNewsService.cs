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
    public async Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await this._messageCache.GetItemAsync(id, cancellationToken);
        return result.Item;
    }

    /// <inheritdoc />
    public async Task<PagedResponse<Story>> GetStoriesAsync(StoryCategory category, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return new()
        {
            Page = page,
            PageSize = pageSize,
            Items = await this._messageCache.GetItemsAsync(category, page, pageSize, cancellationToken),
        };
    }

    /// <inheritdoc />
    public Task<IEnumerable<Story>> FindStory(StoryCategory category, string searchText, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task RemoveItemsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return this._messageCache.RemoveItemsAsync(ids, cancellationToken).AsTask();
    }
}
