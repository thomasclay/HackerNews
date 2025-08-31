using System.Text.Json;

using HackerNewsModels;
using HackerNewsModels.Items;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace HackerNews.ApiService.Services;
/// <summary>
/// Service to cache messages - meant to be a singleton.
/// </summary>
public class MessageCacheService
{
    /// <summary>
    /// Key prefix for item cache entries.
    /// </summary>
    public const string ItemCacheKeyPrefix = "item/";

    /// <summary>
    /// Key prefix for list cache entries.
    /// </summary>
    public const string ListCacheKeyPrefix = "list/";

    private readonly HybridCache _cache;
    private readonly IHackerNewsApi _hackerNewsApi;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<MessageCacheService> _logger;
    private MessageCacheServiceOptions _options;

    /// <summary>
    /// Cache service constructor
    /// </summary>
    /// <param name="cache">Distributed cache to use</param>
    /// <param name="hackerNewsApi">Refit client to access HackerNews</param>
    /// <param name="jsonSerializerOptions">For serializing</param>
    /// <param name="options">Service options</param>
    public MessageCacheService(HybridCache cache,
        IHackerNewsApi hackerNewsApi,
        JsonSerializerOptions jsonSerializerOptions,
        IOptionsMonitor<MessageCacheServiceOptions> options,
        ILogger<MessageCacheService> logger)
    {
        this._cache = cache;
        this._hackerNewsApi = hackerNewsApi;
        this._jsonSerializerOptions = jsonSerializerOptions;
        this._options = options.CurrentValue;
        this._logger = logger;

        options.OnChange(newOptions => this._options = newOptions);
    }

    /// <summary>
    /// Adds or updates a list of story IDs for the given category.
    /// </summary>
    /// <param name="storyCategory">Category list to add/update</param>
    /// <param name="content">Items to add/update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task</returns>
    public ValueTask AddOrUpdateListAsync(StoryCategory storyCategory, long[] content, CancellationToken cancellationToken)
    {
        var serialized = JsonSerializer.SerializeToUtf8Bytes(content, this._jsonSerializerOptions);

        return this._cache.SetAsync(
            $"{ListCacheKeyPrefix}{storyCategory}",
            serialized,
            CreateCacheOptions(),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves a single item
    /// </summary>
    /// <param name="id">Item ID to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Flag indicating if the item was read from the cache, and the item, or null if it doesn't exist.</returns>
    public async Task<(bool FromCache, Item? Item)> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        var state = new ReadState(id, $"{ItemCacheKeyPrefix}{id}",
            CreateCacheOptions());

        var item = await this._cache.GetOrCreateAsync(
            state.Key,
            state,
            ReadFromSource,
            state.Options,
            null,
            cancellationToken: cancellationToken);

        if (state.FromCache)
        {
            ServiceMetrics.ItemCacheHitCounter.Add(1);
        }
        else
        {
            ServiceMetrics.ItemCacheMissCounter.Add(1);
        }
        return (state.FromCache, item);
    }

    /// <summary>
    /// Retrieves a page of stories
    /// </summary>
    /// <param name="category">Category of stories to retrieve</param>
    /// <param name="page">Page number to retrieve</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Flag indicating if the item was read from the cache, and the item, or null if it doesn't exist.</returns>
    public async Task<IEnumerable<Story>> GetStoriesAsync(StoryCategory category, int page, int pageSize, CancellationToken cancellationToken)
    {
        var list = await GetStoryListAsync(category, cancellationToken);
        var items = await GetItemsAsync(list.Skip(page * pageSize).Take(pageSize), cancellationToken);
        return items.OfType<Story>();
    }

    /// <summary>
    /// Retrieves a page of stories
    /// </summary>
    /// <param name="category">Category of stories to retrieve</param>
    /// <param name="page">Page number to retrieve</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Flag indicating if the item was read from the cache, and the item, or null if it doesn't exist.</returns>
    public async Task<IEnumerable<Item>> GetItemsAsync(IEnumerable<long> itemIdentifiers, CancellationToken cancellationToken)
    {
        var items = new List<Task<(bool FromCache, Item? Item)>>();
        foreach (var id in itemIdentifiers)
        {
            items.Add(GetItemAsync(id, cancellationToken));
        }
        var results = await Task.WhenAll(items);

        return results.Where(r => r.Item is not null).Select(r => r.Item!);
    }

    public Task<IEnumerable<long>> GetStoryListAsync(StoryCategory storyCategory, CancellationToken cancellationToken)
    {
        return this._cache.GetOrCreateAsync(
            $"{ListCacheKeyPrefix}{storyCategory}",
            _ => ValueTask.FromResult<IEnumerable<long>>([]),
            cancellationToken: cancellationToken).AsTask();
    }

    /// <summary>
    /// Removes items from the cache.
    /// </summary>
    /// <param name="ids">Item identifiers to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async task</returns>
    public ValueTask RemoveItemsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return this._cache.RemoveAsync(ids.Select(id => $"{ItemCacheKeyPrefix}{id}"), cancellationToken);
    }

    private HybridCacheEntryOptions CreateCacheOptions()
    {
        return new()
        {
            Expiration = TimeSpan.FromHours(this._options.ExpirationInHours),
        };
    }

    private async ValueTask<Item?> ReadFromSource(ReadState state, CancellationToken cancellationToken)
    {
        state.FromCache = false;
        var readItem = await this._hackerNewsApi.GetItemAsync(state.Id, cancellationToken);

        if (!readItem.IsSuccessful)
        {
            ServiceMetrics.ReadItemFailCounter.Add(1);
            this._logger.LogError(readItem.Error.InnerException,
                "Error retrieving item {Id} from source: {Status} {Error}", state.Id, readItem.StatusCode, readItem.Error);
            return null;
        }
        return readItem.Content;
    }

    private class ReadState(long id, string key, HybridCacheEntryOptions options)
    {
        public bool FromCache { get; set; } = true;
        public long Id { get; } = id;
        public string Key { get; } = key;
        public HybridCacheEntryOptions Options { get; } = options;
    }
}