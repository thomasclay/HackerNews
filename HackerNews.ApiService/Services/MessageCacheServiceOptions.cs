namespace HackerNews.ApiService.Services;

/// <summary>
/// Options for items in the message cache.
/// </summary>
public class MessageCacheServiceOptions
{
    /// <summary>
    /// How long messages stay in the cache, in hours. Minimum of 1 hour, maximum of 7 days (168 hours).  Default is 72 hours.
    /// </summary>
    public int ExpirationInHours
    {
        get;
        set => field = Math.Clamp(value, 1, 168);
    } = 72;
}
