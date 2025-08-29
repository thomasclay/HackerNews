namespace HackerNews.ApiService.Services;

public class MessageCacheServiceOptions
{
    private int _expirationInHours = 72;

    /// <summary>
    /// How long messages stay in the cache, in hours. Minimum of 1 hour, maximum of 7 days (168 hours).  Default is 72 hours.
    /// </summary>
    public int ExpirationInHours 
    { 
        get => _expirationInHours; 
        set => _expirationInHours = Math.Clamp(value, 1, 168); 
    }
}
