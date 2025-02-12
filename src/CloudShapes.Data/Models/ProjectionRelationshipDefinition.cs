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

namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the definition of a projection's relationship
/// </summary>
public record ProjectionRelationshipDefinition
{

    /// <summary>
    /// Gets/sets the relationship type
    /// </summary>
    [YamlMember(Alias = "type")]
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the related projection type
    /// </summary>
    [YamlMember(Alias = "target")]
    public virtual string Target { get; set; } = null!;

    /// <summary>
    /// Gets/sets the relationship's foreign key
    /// </summary>
    [YamlMember(Alias = "key")]
    public virtual string Key { get; set; } = null!;

    /// <summary>
    /// Gets/sets the path to the relationship's navigation property<para></para>
    /// It can be the same than <see cref="Key"/>, in which case it replaces the key property with the related projection's state
    /// </summary>
    [YamlMember(Alias = "path")]
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Gets a boolean indicating whether the path to the relationship's foreign key is aligned with the path to its navigation property
    /// </summary>
    [JsonIgnore, BsonIgnore]
    public bool IsForeignKeyPathAligned => Key.Equals(Path);

}