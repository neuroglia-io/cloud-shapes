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
    public override bool CanConvert(Type objectType) => objectType == typeof(ExpandoObject) || objectType == typeof(object);

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject) return ReadObject(reader, serializer);
        else if (reader.TokenType == JsonToken.StartArray) return serializer.Deserialize<List<object>>(reader);
        else return serializer.Deserialize(reader);
    }

    /// <inheritdoc/>
    protected virtual object ReadObject(JsonReader reader, JsonSerializer serializer)
    {
        var dictionary = new Dictionary<string, object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject) return dictionary;
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var propertyName = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(propertyName)) continue;
                reader.Read();
                object? value;
                if (reader.TokenType == JsonToken.StartObject) value = ReadObject(reader, serializer);
                else if (reader.TokenType == JsonToken.StartArray) value = serializer.Deserialize<List<object>>(reader);
                else value = reader.Value;
                dictionary[propertyName] = value!;
            }
        }
        return dictionary;
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

}

