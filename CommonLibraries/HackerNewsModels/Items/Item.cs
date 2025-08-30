using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackerNewsModels.Items;

/// <summary>
/// A base item from hacker news.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Job), "job")]
[JsonDerivedType(typeof(Story), "story")]
[JsonDerivedType(typeof(Comment), "comment")]
[JsonDerivedType(typeof(Poll), "poll")]
[JsonDerivedType(typeof(PollOpt), "pollopt")]
public class Item
{
    /// <summary>
    /// Item ID.
    /// </summary>
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    /// <summary>
    /// Username of story author
    /// </summary>
    [JsonPropertyName("by")]
    public required string By { get; init; }

    /// <summary>
    /// Timestamp of story creation
    /// </summary>
    [JsonPropertyName("time")]
    public required long UnixTime { get; init; }

    [JsonExtensionData()]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
