namespace HackerNews.ApiService.Services;

public class MessageCacheServiceOptions
{
    private int _cacheExpirationHours = 72;

    /// <summary>
    /// How long messages stay in the cach, in hours.  Minimum of 1 hour, maximum of 7 days (168 hours).  Default is 72 hours.
    /// </summary>
    public int CacheExpirationHours
    {
        get => this._cacheExpirationHours;
        set => this._cacheExpirationHours = Math.Clamp(value, 1, 7 * 24);
    }
}
