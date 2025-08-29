using HackerNewsModels;

using Refit;

namespace HackerNews.ApiService.Services;

public interface IHackerNewsApi
{
    [Get("/v0/item/{id}.json")]
    Task<string?> GetStoryAsync(long id, CancellationToken cancellationToken = default);
}
