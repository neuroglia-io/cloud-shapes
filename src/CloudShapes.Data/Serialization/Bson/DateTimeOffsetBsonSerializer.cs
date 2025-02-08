using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CloudShapes.Data.Serialization.Bson;

/// <summary>
/// Represents the <see cref="SerializerBase{TValue}"/> used to serialize/deserialize <see cref="DateTimeOffset"/>s to/from BSON
/// </summary>
public class DateTimeOffsetBsonSerializer 
    : SerializerBase<DateTimeOffset>
{

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset value) => context.Writer.WriteString(value.ToString("o"));

    /// <inheritdoc/>
    public override DateTimeOffset Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return DateTimeOffset.Parse(value);
    }

}
