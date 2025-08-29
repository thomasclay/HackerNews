using Microsoft.Extensions.DependencyInjection;

namespace Common.Utility;

/// <summary>
/// Factory class - singleton used to create instances various types from the service provider.
/// </summary>
public class FactoryService
{
    private readonly IServiceProvider _serviceProvider;

    public FactoryService(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Simply gets the service of type T from the service provider.
    /// </summary>
    /// <returns>The service, if it exists.</returns>
    public T? Get<T>() where T : notnull
    {
        return this._serviceProvider.GetService<T>();
    }

    /// <summary>
    /// Simply gets the service of type T from the service provider.
    /// </summary>
    /// <returns>The service, if it exists.</returns>
    public T GetRequired<T>() where T : notnull
    {
        return this._serviceProvider.GetRequiredService<T>();
    }
}
