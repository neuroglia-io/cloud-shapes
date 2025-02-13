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

namespace CloudShapes.Data.Serialization.Json;

/// <summary>
/// Represents the service used to serialize/deserialize <see cref="Decimal128"/> instances to/from JSON
/// </summary>
public class Decimal128Converter
    : JsonConverter<Decimal128>
{

    /// <inheritdoc/>
    public override Decimal128 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var raw = reader.GetString() ?? throw new JsonException("Expected a non-null string for Decimal128 conversion.");
            try { return Decimal128.Parse(raw); }
            catch (Exception ex) { throw new JsonException($"Error parsing Decimal128 from '{raw}'.", ex); }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            try
            {
                var dec = reader.GetDecimal();
                return new Decimal128(dec);
            }
            catch (Exception ex) { throw new JsonException("Error parsing Decimal128 from numeric value.", ex); }
        }
        else throw new JsonException($"Unexpected token type {reader.TokenType} when parsing Decimal128.");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Decimal128 value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

}