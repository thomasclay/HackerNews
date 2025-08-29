namespace HackerNewsModels;

/// <summary>
/// For retrieving hacker news information
/// </summary>
public interface IHackerNewsService
{
    /// <summary>
    /// Attempts to get the story with the given id
    /// </summary>
    /// <param name="id">Story identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The story, or null</returns>
    public Task<Story?> GetAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Top stories categories.
    /// </summary>
    public enum StoryCategory
    {
        /// <summary>
        /// Newest stories
        /// </summary>
        Newest,
        
        /// <summary>
        /// Top stories
        /// </summary>
        Top,
        
        /// <summary>
        /// Best stories
        /// </summary>
        Best,
    }

    /// <summary>
    /// Retrieves the newest stories, paged.
    /// </summary>
    /// <param name="category">Story category</param>
    /// <param name="page">Page, starting with index 0</param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of stories.</returns>
    public Task<IEnumerable<Story>> LatestAsync(StoryCategory category, int page = 0, int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to find stories with matching text in the title.
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any stories where the title matches</returns>
    public Task<IEnumerable<Story>> FindAsync(string search, int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default);
}
