using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackerNewsModels;

/// <summary>
/// List of items and profiles that have changed recently. 
/// </summary>
public class UpdatedItems
{
    /// <summary>
    /// Item identifiers that have changed recently.
    /// </summary>
    [JsonPropertyName("items")]
    public long[] Items { get; init; } = [];

    /// <summary>
    /// Profile names that have changed recently.
    /// </summary>
    [JsonPropertyName("profiles")]
    public string[] Profiles { get; init; } = [];

    [JsonExtensionData()]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
