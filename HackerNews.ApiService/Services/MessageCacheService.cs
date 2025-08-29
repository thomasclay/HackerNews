using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using HackerNewsModels.Items;
using System.Text.Json;

namespace HackerNews.ApiService.Services;

/// <summary>
/// Service to cache messages - meant to be a singleton.
/// </summary>
public class MessageCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IHackerNewsApi _hackerNewsApi;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private MessageCacheServiceOptions _options;

    public MessageCacheService(IDistributedCache cache,
        IHackerNewsApi hackerNewsApi,
        JsonSerializerOptions jsonSerializerOptions,
        IOptionsMonitor<MessageCacheServiceOptions> options)
    {
        this._cache = cache;
        this._hackerNewsApi = hackerNewsApi;
        this._jsonSerializerOptions = jsonSerializerOptions;
        this._options = options.CurrentValue;

        options.OnChange(newOptions =>
        {
            this._options = newOptions;
        });
    }

    public async Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        var key = $"item-{id}";
        var itemBytes = await this._cache.GetAsync(key, cancellationToken);
        if (itemBytes is null)
        {
            var readItem = await this._hackerNewsApi.GetItemAsync(id, cancellationToken);
            if (!readItem.IsSuccessful)
            {
                return null;
            }
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(this._options.ExpirationInHours)
            };
            var serializedItem = JsonSerializer.Serialize(readItem.Content, this._jsonSerializerOptions);
            await this._cache.SetStringAsync(key, serializedItem, options, cancellationToken);
            return readItem.Content;
        }
        return JsonSerializer.Deserialize<Item>(itemBytes, this._jsonSerializerOptions);
    }
}
