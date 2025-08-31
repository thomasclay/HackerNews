using Quartz;

using HackerNews.ApiService.Services;

namespace HackerNews.ApiService.Jobs;

/// <summary>
/// Job to get changed items from Hacker News API and remove them from the cache.
/// </summary>
public class UpdatedItemsJob(IHackerNewsApi newsApi, MessageCacheService cache, ILogger<UpdatedItemsJob> logger) : IJob
{
    private readonly IHackerNewsApi _newsApi = newsApi;
    private readonly MessageCacheService _cache = cache;
    private readonly ILogger<UpdatedItemsJob> _logger = logger;

    /// <summary>
    /// Quartz job key
    /// </summary>
    public static readonly JobKey Key = new(nameof(UpdatedItemsJob));

    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        ServiceMetrics.ChangeUpdateCounter.Add(1);

        var changedItems = await this._newsApi.GetUpdatesAsync(context.CancellationToken);
        if (!changedItems.IsSuccessful)
        {
            this._logger.LogError("Failed to get updated items from Hacker News API. Status code: {StatusCode} {@Error}",
                changedItems.StatusCode,
                changedItems.Error);

            return;
        }

        await this._cache.RemoveItemsAsync(changedItems.Content.Items, context.CancellationToken);
    }
}
