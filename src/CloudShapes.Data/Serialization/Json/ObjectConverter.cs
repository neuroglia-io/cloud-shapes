// Copyright © 2025-Present The Cloud Shapes Authors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Globalization;

namespace CloudShapes.Data.Serialization.Json;

/// <summary>
/// Represents the service used to serialize/deserialize objects to/from JSON
/// </summary>
public class ObjectConverter 
    : JsonConverter<object>
{

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(object) || typeof(IDictionary<string, object>).IsAssignableFrom(typeToConvert);

    /// <inheritdoc/>
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => ReadValue(ref reader);

    static object? ReadValue(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                return ReadObject(ref reader);
            case JsonTokenType.StartArray:
                return ReadArray(ref reader);
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var l)) return l;
                return reader.GetDouble();
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Null:
                return null;
            default:
                ThrowJsonException(reader.TokenType);
                return null;
        }
    }

    static Dictionary<string, object> ReadObject(ref Utf8JsonReader reader)
    {
        var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return dictionary;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString()!;
                reader.Read();
                dictionary[propertyName] = ReadValue(ref reader)!;
            }
        }
        ThrowJsonException(JsonTokenType.StartObject);
        return dictionary;
    }

    private static List<object> ReadArray(ref Utf8JsonReader reader)
    {
        var list = new List<object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return list;

            list.Add(ReadValue(ref reader)!);
        }
        ThrowJsonException(JsonTokenType.StartArray);
        return list;
    }

    static void ThrowJsonException(JsonTokenType tokenType) => throw new JsonException($"Invalid JSON structure: Expected {tokenType}");

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case IEnumerable<object> list:
                writer.WriteStartArray();
                foreach (var item in list) Write(writer, item, options);
                writer.WriteEndArray();
                break;
            case string s:
                writer.WriteStringValue(s);
                break;
            case short s:
                writer.WriteNumberValue(s);
                break;
            case int i:
                writer.WriteNumberValue(i);
                break;
            case long l:
                writer.WriteNumberValue(l);
                break;
            case double d:
                writer.WriteNumberValue(d);
                break;
            case Decimal128 d:
                var raw = d.ToString();
                if (decimal.TryParse(raw, CultureInfo.InvariantCulture, out var @decimal)) writer.WriteNumberValue(@decimal);
                else writer.WriteNumberValue(double.Parse(raw));
                break;
            case bool b:
                writer.WriteBooleanValue(b);
                break;
            case null:
                writer.WriteNullValue();
                break;
            case IDictionary<string, object>:
            default:
                var t = value.GetType();
                if (value is not IDictionary<string, object> dictionary) dictionary = value.ToDictionary()!;
                writer.WriteStartObject();
                foreach (var kvp in dictionary)
                {
                    writer.WritePropertyName(kvp.Key);
                    Write(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
                break;
        }
    }

}
