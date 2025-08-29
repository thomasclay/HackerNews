using Microsoft.Extensions.Caching.Distributed;

using Quartz;

using HackerNewsModels;

using HackerNews.ApiService.Services;
using System.Text.Json;

namespace HackerNews.ApiService.Jobs;

/// <summary>
/// Job to get changed items from Hacker News API and remove them from the cache.
/// </summary>
public class GetListsJob : IJob
{
    private readonly IHackerNewsApi _newsApi;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UpdatedItemsJob> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public static readonly JobKey Key = new(nameof(GetListsJob));

    public GetListsJob(IHackerNewsApi newsApi, IDistributedCache cache, JsonSerializerOptions jsonSerializerOptions, ILogger<UpdatedItemsJob> logger)
    {
        this._newsApi = newsApi;
        this._cache = cache;
        this._jsonSerializerOptions = jsonSerializerOptions;
        this._logger = logger; ;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var categoryTasks = new Dictionary<StoryCategory, Task<Refit.ApiResponse<long[]>>>()
        {
            [StoryCategory.Top] = this._newsApi.GetTopItemIdentifierAsync(context.CancellationToken),
            [StoryCategory.Best] = this._newsApi.GetBestItemIdentifiersAsync(context.CancellationToken),
            [StoryCategory.New] = this._newsApi.GetNewItemIdentifiersAsync(context.CancellationToken),
        };
        await Task.WhenAll(categoryTasks.Values);

        var tasks = new List<Task>();
        foreach (var (category, apiResult) in categoryTasks)
        {
            if (!apiResult.Result.IsSuccessful)
            {
                this._logger.LogError("Failed to get {Category} items from Hacker News API. Status code: {StatusCode} {@Error}",
                    category,
                    apiResult.Result.StatusCode,
                    apiResult.Result.Error);
                continue;
            }
            var key = $"list-{category}";
            var serialized = JsonSerializer.SerializeToUtf8Bytes(apiResult.Result.Content, this._jsonSerializerOptions);
            tasks.Add(_cache.SetAsync(key, serialized, context.CancellationToken));
        }
        await Task.WhenAll(tasks);
    }
}
