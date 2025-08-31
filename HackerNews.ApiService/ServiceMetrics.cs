using System.Diagnostics.Metrics;

namespace HackerNews.ApiService;

public class ServiceMetrics
{

    public static readonly Meter ServiceMeter = new(nameof(ServiceMetrics));

    public static readonly Counter<int> ListUpdateCounter = ServiceMeter.CreateCounter<int>("list.update.count", description: "Counter for top/best/new lists were retrieved.");
    public static readonly Counter<int> ChangeUpdateCounter = ServiceMeter.CreateCounter<int>("change.update.count", description: "Counter for changes retrieved.");
    public static readonly Counter<int> ReadItemFailCounter = ServiceMeter.CreateCounter<int>("read.item.failure.count", description: "Counter of number of failures retrieving items from HackerNews.");
    public static readonly Counter<int> ItemCacheHitCounter = ServiceMeter.CreateCounter<int>("item.cache.hit.count", description: "Count of item cache hits.");
    public static readonly Counter<int> ItemCacheMissCounter = ServiceMeter.CreateCounter<int>("item.cache.miss.count", description: "Count of item cache misses.");
}
