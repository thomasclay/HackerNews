using System.Text.Json.Serialization;

namespace HackerNewsModels.Items;

/// <summary>
/// Information about a single story.
/// </summary>
public class Story : Item
{
    /// <summary>
    /// Flag indicating if the item is deleted
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool? Deleted { get; init; }

    /// <summary>
    /// Child items (comments) of a story.
    /// </summary>
    [JsonPropertyName("descendants")]
    public required int Descendants { get; init; }

    /// <summary>
    /// Identifiers of child comments, in ranked display order.
    /// </summary>
    [JsonPropertyName("kids")]
    public required long[] Kids { get; init; }

    /// <summary>
    /// Story score (points)
    /// </summary>
    [JsonPropertyName("score")]
    public required long Score { get; init; }

    /// <summary>
    /// Title of the story
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// Optional URL of the story
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
}