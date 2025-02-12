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
/// Represents a projection type
/// </summary>
public record ProjectionType
{

    /// <summary>
    /// Initializes a new <see cref="ProjectionType"/>
    /// </summary>
    public ProjectionType() { }

    /// <summary>
    /// Initializes a new <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="name">The projection type's name</param>
    /// <param name="schema">The schema that defines, documents and validates the state of projections of this type</param>
    /// <param name="triggers">A list containing the triggers responsible for creating new projections when specific CloudEvents occur</param>
    /// <param name="indexes">A list containing the indexes, if any, of projections of this type</param>
    /// <param name="relationships">list containing the relationships, if any, of projections of this type</param>
    /// <param name="summary">A short, concise description of the projection type</param>
    /// <param name="description">The projection type's description, if any</param>
    /// <param name="tags">A key/value mapping of the tags, if any, associated to the projection type</param>
    public ProjectionType(string name, JsonSchema schema, ProjectionTriggerCollection triggers, IEnumerable<ProjectionIndexDefinition>? indexes = null, IEnumerable<ProjectionRelationshipDefinition>? relationships = null, string? summary = null, string? description = null, IDictionary<string, string>? tags = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(triggers);
        Name = name;
        Schema = schema;
        Triggers = triggers;
        Indexes = indexes == null ? null : new(indexes);
        Relationships = relationships == null ? null : new(relationships);
        Summary = summary;
        Description = description;
        Tags = tags == null ? null : new(tags);
    }

    /// <summary>
    /// Gets/sets the <see cref="ProjectionType"/>'s metadata
    /// </summary>
    [YamlMember(Alias = "metadata")]
    public virtual ProjectionTypeMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Gets/sets the projection type's name
    /// </summary>
    [BsonId, YamlMember(Alias = "name")]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets a short, concise description of the projection type<para></para>
    /// Supports Markdown
    /// </summary>
    [YamlMember(Alias = "summary")]
    public virtual string? Summary { get; set; }

    /// <summary>
    /// Gets/sets the projection type's description, if any<para></para>
    /// Supports Markdown
    /// </summary>
    [YamlMember(Alias = "description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets/sets the schema that defines, documents and validates the state of projections of this type
    /// </summary>
    [YamlMember(Alias = "schema")]
    public virtual JsonSchema Schema { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    [YamlMember(Alias = "triggers")]
    public virtual ProjectionTriggerCollection Triggers { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the indexes, if any, of projections of this type
    /// </summary>
    [YamlMember(Alias = "indexes")]
    public virtual EquatableList<ProjectionIndexDefinition>? Indexes { get; set; }

    /// <summary>
    /// Gets/sets a list containing the relationships, if any, of projections of this type
    /// </summary>
    [YamlMember(Alias = "relationships")]
    public virtual EquatableList<ProjectionRelationshipDefinition>? Relationships { get; set; }

    /// <summary>
    /// Gets/sets a key/value mapping of the tags, if any, associated to the projection type
    /// </summary>
    [YamlMember(Alias = "tags")]
    public virtual EquatableDictionary<string, string>? Tags { get; set; }

}
