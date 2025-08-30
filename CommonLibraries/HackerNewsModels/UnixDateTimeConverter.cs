using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackerNewsModels;

/// <summary>
/// User to convert Unix time to and from DateTime.
/// </summary>
public class UnixDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var a = reader.GetString();
        return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).UtcDateTime;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime dateTimeValue,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(new DateTimeOffset(dateTimeValue).ToUnixTimeSeconds().ToString());
}