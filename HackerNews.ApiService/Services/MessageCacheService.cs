using HackerNewsModels;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNews.ApiService.Services;

/// <summary>
/// Service - meant to be a singleton.
/// </summary>
public class MessageCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IHackerNewsApi _hackerNewsApi;
    private readonly LoadedStories _loadedStories;
    private MessageCacheServiceOptions _options;

    public MessageCacheService(IMemoryCache cache,
        IHackerNewsApi hackerNewsApi,
        IOptionsMonitor<MessageCacheServiceOptions> options,
        LoadedStories loadedStories)
    {
        this._cache = cache;
        this._hackerNewsApi = hackerNewsApi;
        this._options = options.CurrentValue;
        this._loadedStories = loadedStories;

        options.OnChange(newOptions =>
        {
            this._options = newOptions;
        });
    }

    public async Task<Story?> GetStoryAsync(long id, CancellationToken cancellationToken)
    {
        var key = $"story-{id}";
        var story = await this._cache.GetOrCreateAsync(key, async entry =>
        {
            return AddStoryInternal(entry, id, await this._hackerNewsApi.GetStoryAsync(id, cancellationToken));
        });
        return story;
    }

    public async Task<long[]> GetLiveStoryIdsAsync(IHackerNewsService.StoryCategory category, CancellationToken cancellationToken)
    {
        return await this._cache.GetOrCreateAsync($"live-{category}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(this._options.LiveStoryExpirationMinutes);
            return await this._hackerNewsApi.GetLiveStoryIdsAsync(category.ToString().ToLower(), cancellationToken);
        }) ?? Array.Empty<long>();
    }

    public void AddStory(Story story)
    {
        AddStoryInternal(this._cache.CreateEntry($"story-{story.Id}"), story.Id, story);
    }

    private Story? AddStoryInternal(ICacheEntry entry, long id, Story? story)
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(this._options.CacheExpirationHours);
        entry.RegisterPostEvictionCallback(EjectingItem, id);
        if (story is not null)
        {
            this._loadedStories.Add(id);
        }
        return story;
    }

    private void EjectingItem(object key, object? value, EvictionReason reason, object? state)
    {
        if (state is int id)
        {
            this._loadedStories.Remove(id);
        }
    }
}
