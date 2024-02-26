using System.Text.Json;
using System.Text.Json.Serialization;
namespace Cards.WebAPI.Models.Configuration;

/// <summary>
/// Custom converter for JSON serialization of empty string to null. Allows better handling with Convert functions. E.g. Convert.ToInt64()
/// </summary>
public class EmptyStringToNullConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        return value == string.Empty ? null : value?.Trim();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}