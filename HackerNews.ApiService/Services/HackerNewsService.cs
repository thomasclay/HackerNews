using HackerNewsModels;
using HackerNewsModels.Items;

namespace HackerNews.ApiService.Services;

public class HackerNewsService : IHackerNewsService
{
    private readonly MessageCacheService _messageCache;

    public HackerNewsService(MessageCacheService messageCache)
    {
        this._messageCache = messageCache;
    }

    /// <inheritdoc />
    public Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken = default)
    {
        return this._messageCache.GetItemAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PagedResponse<Story>> GetStoriesAsync(StoryCategory category, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IEnumerable<Story>> FindAsync(StoryCategory category, string searchText, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
