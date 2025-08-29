using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackerNewsModels;


public class UnixDateTimeJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset dateTimeValue,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(dateTimeValue.ToUnixTimeSeconds().ToString());
}