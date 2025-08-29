using System.Text;
using System.Text.Json;

using HackerNewsModels;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace HackerNews.ApiService.Services;

/// <summary>
/// Service - meant to be a singleton.
/// </summary>
public class MessageCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IHackerNewsApi _hackerNewsApi;
    private MessageCacheServiceOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public MessageCacheService(IDistributedCache cache, IHackerNewsApi hackerNewsApi, IOptionsMonitor<MessageCacheServiceOptions> options, JsonSerializerOptions jsonOptions)
    {
        this._cache = cache;
        this._hackerNewsApi = hackerNewsApi;
        this._options = options.CurrentValue;
        this._jsonOptions = jsonOptions;
        options.OnChange(newOptions => this._options = newOptions);
    }

    public async Task<Story?> GetStoryAsync(long id, CancellationToken cancellationToken)
    {
        var key = $"story-{id}";
        
        var storyString = await this._cache.GetStringAsync(key, cancellationToken)
            ?? await LoadStoryStringAsync(key, id, cancellationToken);

        return JsonSerializer.Deserialize<Story>(storyString ?? string.Empty, this._jsonOptions);
    }

    private async Task<string?> LoadStoryStringAsync(string key, long id, CancellationToken cancellationToken)
    {
        var item = await this._hackerNewsApi.GetStoryAsync(id, cancellationToken);

        if (item is not null)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(this._options.CacheExpirationHours),
            };
            await this._cache.SetAsync(key, Encoding.UTF8.GetBytes(item), options, cancellationToken);
        }

        return item;
    }
}
