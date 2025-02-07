using Newtonsoft.Json;
using System.Dynamic;

namespace CloudShapes.Data.Serialization.Json;

/// <summary>
/// Represents the <see cref="JsonConverter"/> used to serialize/deserialize objects and <see cref="ExpandoObject"/>s
/// </summary>
public class ObjectConverter 
    : JsonConverter
{

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) => objectType == typeof(object) || objectType == typeof(IDictionary<string, object>);

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject) return ReadObject(reader, serializer);
        else if (reader.TokenType == JsonToken.StartArray) return ReadArray(reader, serializer);
        else return serializer.Deserialize(reader);
    }

    /// <summary>
    /// Reads and converts a JSON object into an IDictionary<string, object>.
    /// </summary>
    protected virtual IDictionary<string, object> ReadObject(JsonReader reader, JsonSerializer serializer)
    {
        var dictionary = new Dictionary<string, object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)  return dictionary;
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(propertyName)) continue;
                reader.Read();
                dictionary[propertyName] = ReadValue(reader, serializer);
            }
        }
        return dictionary;
    }

    /// <summary>
    /// Reads and converts a JSON array into a List<object>, recursively converting JObjects inside.
    /// </summary>
    protected virtual List<object> ReadArray(JsonReader reader, JsonSerializer serializer)
    {
        var list = new List<object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndArray) return list;
            list.Add(ReadValue(reader, serializer));
        }
        return list;
    }

    /// <summary>
    /// Recursively reads a JSON value, converting nested objects to dictionaries and arrays to lists.
    /// </summary>
    private object ReadValue(JsonReader reader, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject) return ReadObject(reader, serializer);
        else if (reader.TokenType == JsonToken.StartArray) return ReadArray(reader, serializer);
        else return serializer.Deserialize(reader)!;
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => serializer.Serialize(writer, value);

}
