﻿// Copyright © 2025-Present The Cloud Shapes Authors
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

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CloudShapes.Data.Serialization.Bson;

/// <summary>
/// Represents the <see cref="SerializerBase{TValue}"/> used to serialize/deserialize <see cref="decimal"/>s to/from BSON
/// </summary>
public class DecimalBsonSerializer
    : SerializerBase<decimal>
{

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, decimal value) => context.Writer.WriteDouble((double)value);

    /// <inheritdoc/>
    public override decimal Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => (decimal)context.Reader.ReadDouble();

}
