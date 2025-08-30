using HackerNewsModels;
using HackerNewsModels.Items;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

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

    /// <summary>
    /// Key prefix for item cache entries.
    /// </summary>
    public const string ItemCacheKeyPrefix = "item-";

    /// <summary>
    /// Key prefix for list cache entries.
    /// </summary>
    public const string ListCacheKeyPrefix = "list-";

    /// <summary>
    /// Cache service constructor
    /// </summary>
    /// <param name="cache">Distributed cache to use</param>
    /// <param name="hackerNewsApi">Refit client to access HackerNews</param>
    /// <param name="jsonSerializerOptions">For serializing</param>
    /// <param name="options">Service options</param>
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

    /// <summary>
    /// Retrieves a single item
    /// </summary>
    /// <param name="id">Item ID to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item, or null if it doesn't exist.</returns>
    public async Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        var key = ItemCacheKeyPrefix + id.ToString();
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
            var serializedItem = JsonSerializer.SerializeToUtf8Bytes(readItem.Content, this._jsonSerializerOptions);
            await this._cache.SetAsync(key, serializedItem, options, cancellationToken);
            return readItem.Content;
        }
        return JsonSerializer.Deserialize<Item>(itemBytes, this._jsonSerializerOptions);
    }

    /// <summary>
    /// Removes items from the cache.
    /// </summary>
    /// <param name="ids">Item identifiers to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task</returns>
    public async Task RemoveItemsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        foreach (var id in ids)
        {
            var key = ItemCacheKeyPrefix + id.ToString();
            tasks.Add(this._cache.RemoveAsync(key, cancellationToken));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Adds or updates a list of story IDs for the given category.
    /// </summary>
    /// <param name="storyCategory">Category list to add/update</param>
    /// <param name="content">Items to add/update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task</returns>
    public Task AddOrUpdateListAsync(StoryCategory storyCategory, long[] content, CancellationToken cancellationToken)
    {
        var serialized = JsonSerializer.SerializeToUtf8Bytes(content, this._jsonSerializerOptions);

        return this._cache.SetAsync(
            ListCacheKeyPrefix + storyCategory.ToString(),
            serialized,
            cancellationToken);
    }

}
