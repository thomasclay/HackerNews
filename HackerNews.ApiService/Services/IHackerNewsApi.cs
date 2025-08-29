using HackerNewsModels;

using Refit;

namespace HackerNews.ApiService.Services;

public interface IHackerNewsApi
{
    [Get("/v0/item/{id}.json")]
    Task<Story?> GetStoryAsync(long id, CancellationToken cancellationToken = default);

    [Get("/v0/{category}stories.json")]
    Task<long[]> GetLiveStoryIdsAsync(string category, CancellationToken cancellationToken = default);
}
