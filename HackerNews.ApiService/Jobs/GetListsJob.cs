using Quartz;

using HackerNewsModels;

using HackerNews.ApiService.Services;

namespace HackerNews.ApiService.Jobs;

/// <summary>
/// Job to get changed items from Hacker News API and remove them from the cache.
/// </summary>
public class GetListsJob(IHackerNewsApi newsApi, MessageCacheService cache, ILogger<UpdatedItemsJob> logger) : IJob
{
    private readonly IHackerNewsApi _newsApi = newsApi;
    private readonly MessageCacheService _cache = cache;
    private readonly ILogger<UpdatedItemsJob> _logger = logger;

    /// <summary>
    /// Quartz job key
    /// </summary>
    public static readonly JobKey Key = new(nameof(GetListsJob));

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        ServiceMetrics.ListUpdateCounter.Add(1);

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
            tasks.Add(this._cache.AddOrUpdateListAsync(category, apiResult.Result.Content, context.CancellationToken).AsTask());
        }
        await Task.WhenAll(tasks);
    }
}
