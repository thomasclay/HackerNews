using HackerNewsModels;
using HackerNewsModels.Items;

using Microsoft.AspNetCore.Mvc;

namespace HackerNews.ApiService.Controllers;

/// <summary>
/// Primary Items controller
/// </summary>
/// <param name="service">Hacker news service</param>
[Route("api/items")]
[ApiController()]
public class ItemsController(IHackerNewsService service) : ControllerBase
{
    private readonly IHackerNewsService _service = service;

    /// <summary>
    /// Retrieves the item with the given id, or null if not found.
    /// </summary>
    /// <param name="id">Id of the item to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A story, or null of not found</returns>
    [HttpGet("{id:long}")]
    public Task<Item?> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        return this._service.GetItemAsync(id, cancellationToken);
    }
}
