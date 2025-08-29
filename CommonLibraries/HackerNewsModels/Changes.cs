namespace HackerNewsModels;

/// <summary>
/// List of changes that have changed recently. 
/// </summary>
/// <param name="Items">Item identifiers that have changed.</param>
/// <param name="Profiles">Profile identifiers that have changed.</param>
public record Changes(IEnumerable<long> Items, IEnumerable<long> Profiles);
