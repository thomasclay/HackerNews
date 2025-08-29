using HackerNewsModels;
using HackerNewsModels.Items;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HackerNews.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController()]
public class StoriesController : ControllerBase
{
    private readonly IHackerNewsService _service;

    public StoriesController(IHackerNewsService service)
    {
        this._service = service;
    }

    /// <summary>
    /// Retrieves the story with the given id, or null if not found.
    /// </summary>
    /// <param name="id">Id of the story to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A story, or null of not found or if id is not a story.</returns>
    [HttpGet("story/{id:long}")]
    public async Task<Story?> GetStoryAsync(long id, CancellationToken cancellationToken)
    {
        return (await this._service.GetItemAsync(id, cancellationToken)) as Story;
    }

    /// <summary>
    /// Gets the latest stories.
    /// </summary>
    /// <param name="page">Page, starting with 0</param>
    /// <param name="pageSize">Size of page, clamped to 1..50</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged response of stories</returns>
    [HttpGet("stories/{category}")]
    public Task<PagedResponse<Story>> GetStoriesAsync(StoryCategory category, int page = 0, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return this._service.GetStoriesAsync(category, page, pageSize, cancellationToken);
    }

    [HttpGet("find/{category}")]
    public Task<IEnumerable<Story>> FindAsync(StoryCategory category, [BindRequired()] string search, CancellationToken cancellationToken = default)
    {
        return this._service.FindAsync(category, search, cancellationToken);
    }
}
