using HackerNewsModels;

using Microsoft.Extensions.Caching.Distributed;

namespace HackerNews.ApiService.Services;

public class HackerNewsService : IHackerNewsService
{
    private readonly MessageCacheService _messageCache;

    public HackerNewsService(MessageCacheService messageCache)
    {
        this._messageCache = messageCache;
    }

    /// <inheritdoc />
    public Task<Story?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        return this._messageCache.GetStoryAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Story>> LatestAsync(IHackerNewsService.StoryCategory category, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IEnumerable<Story>> FindAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
