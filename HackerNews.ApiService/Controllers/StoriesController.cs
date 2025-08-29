using HackerNewsModels;

using Microsoft.AspNetCore.Mvc;

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
    /// <returns>A story, or null of not found.</returns>
    [HttpGet("{id:long}")]
    public Task<Story?> GetAsync(long id)
    {
        return this._service.GetAsync(id);
    }
    
    [HttpGet("latest/{category}")]
    public Task<IEnumerable<Story>> LatestAsync(IHackerNewsService.StoryCategory category, int page = 0, int pageSize = 10)
    {
        return this._service.LatestAsync(category, page, pageSize);
    }
    
    [HttpGet("find")]
    public Task<IEnumerable<Story>> FindAsync(string search, int page = 1, int pageSize = 10)
    {
        return this._service.FindAsync(search, page, pageSize);
    }
}
