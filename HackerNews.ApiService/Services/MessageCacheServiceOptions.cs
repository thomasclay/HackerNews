namespace HackerNews.ApiService.Services;

public class MessageCacheServiceOptions
{
    /// <summary>
    /// How long messages stay in the cache, in hours.  Minimum of 1 hour, maximum of 7 days (168 hours).  Default is 72 hours.
    /// </summary>
    public int CacheExpirationHours { get; set; } = 72;

    public int LiveStoryExpirationMinutes { get; set; } = 5;
}
