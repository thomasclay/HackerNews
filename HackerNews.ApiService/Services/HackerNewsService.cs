using HackerNewsModels;

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
    public Task<IEnumerable<Story>> LatestStoriesAsync(IHackerNewsService.StoryCategory category, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<long[]> LatestIdsAsync(IHackerNewsService.StoryCategory category, CancellationToken cancellationToken)
    {
        return this._messageCache.GetLiveStoryIdsAsync(category, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Story>> FindAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
