using System.Text.Json.Serialization;

namespace HackerNewsModels;

/// <summary>
/// 
/// </summary>
public class Story
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("by")]
    public required string By { get; init; }

    [JsonPropertyName("descendants")]
    public required int Descendants { get; init; }

    [JsonPropertyName("kids")]
    public required IEnumerable<long> Kids { get; init; }

    [JsonPropertyName("score")]
    public required long Score { get; init; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(UnixDateTimeJsonConverter))]
    public required DateTimeOffset Time { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("type")]
    public required string Url { get; init; }
}