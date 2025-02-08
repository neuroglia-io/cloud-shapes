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
        var document = BsonDocumentSerializer.Instance.Deserialize(context);
        var json = document.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson });
        return JsonSchema.FromText(json);
    }

}
