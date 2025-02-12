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

using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CloudShapes.Data.Serialization.Bson;

/// <summary>
/// Represents the <see cref="SerializerBase{TValue}"/> used to serialize/deserialize <see cref="JsonSchema"/>s to/from BSON
/// </summary>
public class JsonSchemaBsonSerializer
    : SerializerBase<JsonSchema>
{

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonSchema value)
    {
        if (value == null)  context.Writer.WriteNull();
        else BsonDocumentSerializer.Instance.Serialize(context, BsonDocument.Parse(JsonSerializer.Serialize(value)));
    }

    /// <inheritdoc/>
    public override JsonSchema Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null) return null!;
        var document = BsonDocumentSerializer.Instance.Deserialize(context);
        var json = document.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson });
        return JsonSchema.FromText(json);
    }

}
