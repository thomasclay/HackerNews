using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Utility;

public static class CachingExtensions
{
    public static IHostApplicationBuilder AddCaching(this IHostApplicationBuilder builder, string name, CachingType cacheType)
    {
        switch (cacheType)
        {
            case CachingType.InMemory:
                builder.Services.AddMemoryCache();
                break;
            case CachingType.Redis:
                builder.AddRedisDistributedCache(name);
                break;
            default:
                throw new NotSupportedException($"Caching type {cacheType} is not supported.");
        }
        builder.Services.AddHybridCache();
        return builder;
    }
}
