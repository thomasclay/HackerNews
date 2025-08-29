using HackerNewsModels;

using Quartz;

namespace HackerNews.ApiService.Jobs;

public class GetStories : IJob
{
    readonly ILogger<GetStories> _logger;

    public GetStories(ILogger<GetStories> logger)
    {
        this._logger = logger; ;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var category = GetCategory(context.MergedJobDataMap["category"]);
        this._logger.LogInformation("Getting stories for category {category}", category);
        return Task.CompletedTask;
    }

    private IHackerNewsService.StoryCategory GetCategory(object obj)
    {
        return Enum.TryParse<IHackerNewsService.StoryCategory>(obj.ToString(), out var category)
            ? category
            : throw new ArgumentException($"Invalid category value {obj}", nameof(obj));
    }
}
