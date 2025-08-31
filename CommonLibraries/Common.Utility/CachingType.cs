namespace Common.Utility;

/// <summary>
/// What type of caching to use. Only set during startup.
/// </summary>
public enum CachingType
{
    /// <summary>
    /// Use the in-memory cache as the backing cache.
    /// </summary>
    InMemory,

    /// <summary>
    /// Use a Redis cache as the backing cache.
    /// </summary>
    Redis,
}
