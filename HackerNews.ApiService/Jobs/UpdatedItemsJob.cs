using Microsoft.Extensions.Caching.Distributed;

using Quartz;

using HackerNews.ApiService.Services;

namespace HackerNews.ApiService.Jobs;

/// <summary>
/// Job to get changed items from Hacker News API and remove them from the cache.
/// </summary>
public class UpdatedItemsJob : IJob
{
    private readonly IHackerNewsApi _newsApi;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UpdatedItemsJob> _logger;

    public static readonly JobKey Key = new(nameof(UpdatedItemsJob));

    public UpdatedItemsJob(IHackerNewsApi newsApi, IDistributedCache cache, ILogger<UpdatedItemsJob> logger)
    {
        this._newsApi = newsApi;
        this._cache = cache;
        this._logger = logger; ;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var changedItems = await this._newsApi.GetUpdatesAsync(context.CancellationToken);
        if (!changedItems.IsSuccessful)
        {
            this._logger.LogError("Failed to get updated items from Hacker News API. Status code: {StatusCode} {@Error}", 
                changedItems.StatusCode,
                changedItems.Error);
            return;
        }
        var tasks = new List<Task>();
        foreach (var itemId in changedItems.Content.Items)
        {
            tasks.Add(this._cache.RemoveAsync($"story-{itemId}", context.CancellationToken));
        }
        await Task.WhenAll(tasks);
    }
}
