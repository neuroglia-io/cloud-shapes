using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudShapes.Data.Serialization.Bson;

/// <summary>
/// Represents the <see cref="SerializerBase{TValue}"/> used to serialize objects
/// </summary>
public class ObjectSerializer
    : SerializerBase<object>
{

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value is not JToken token)
        {
            BsonDocumentSerializer.Instance.Serialize(context, BsonValue.Create(value));
            return;
        }
        if (token == null || token.Type == JTokenType.Null)
        {
            context.Writer.WriteNull();
            return;
        }

        switch (token.Type)
        {
            case JTokenType.Object:
                var document = BsonDocument.Parse(token.ToString());
                BsonDocumentSerializer.Instance.Serialize(context, document);
                break;

            case JTokenType.Array:
                var array = new BsonArray(((JArray)token).Select(JTokenToBsonValue)); // Convert JArray properly
                BsonArraySerializer.Instance.Serialize(context, array);
                break;

            default:
                var primitive = JTokenToBsonValue(token);
                BsonValueSerializer.Instance.Serialize(context, primitive);
                break;
        }
    }

    /// <inheritdoc/>
    public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = BsonValueSerializer.Instance.Deserialize(context);
        var json = value.ToJson(new() { OutputMode = MongoDB.Bson.IO.JsonOutputMode.RelaxedExtendedJson });
        return JsonConvert.DeserializeObject<object>(json)!;
    }

    /// <summary>
    /// Converts a JToken to a BSON Value.
    /// </summary>
    protected virtual BsonValue JTokenToBsonValue(JToken token)
    {
        return token.Type switch
        {
            JTokenType.Object => BsonDocument.Parse(token.ToString()),
            JTokenType.Array => new BsonArray(((JArray)token).Select(JTokenToBsonValue)),
            _ => BsonValue.Create(token.ToObject<object>())
        };
    }

}
