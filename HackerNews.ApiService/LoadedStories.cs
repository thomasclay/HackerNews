namespace HackerNews.ApiService;

/// <summary>
/// Keeps track of loaded story identifiers
/// </summary>
public class LoadedStories
{
    private SortedList<long, long> _stories;
    private readonly object _lock;
    public LoadedStories()
    {
        this._stories = [];
        this._lock = new();
    }

    public void Add(long id)
    {
        lock (this._lock)
        {
            if (!this._stories.ContainsKey(id))
            {
                this._stories.Add(id, id);
            }
        }
    }

    public bool Remove(long id)
    {
        lock (this._lock)
        {
            return this._stories.Remove(id);
        }
    }
}
