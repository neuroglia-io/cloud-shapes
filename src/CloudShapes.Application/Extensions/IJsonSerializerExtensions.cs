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

namespace CloudShapes.Application;

/// <summary>
/// Defines extensions for <see cref="IJsonSerializer"/>s
/// </summary>
public static class IJsonSerializerExtensions
{

    /// <summary>
    /// Serializes the specified value as a <see cref="BsonDocument"/>
    /// </summary>
    /// <param name="serializer">The extended <see cref="IJsonSerializer"/></param>
    /// <param name="graph">The value to serialize</param>
    /// <returns>The deserialized <see cref="BsonDocument"/></returns>
    public static BsonDocument? SerializeToBsonDocument(this IJsonSerializer serializer, object? graph)
    {
        if (graph == null) return null;
        var json = serializer.SerializeToText(graph);
        return BsonDocument.Parse(json);
    }

}
