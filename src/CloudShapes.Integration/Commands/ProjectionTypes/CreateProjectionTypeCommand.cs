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

namespace CloudShapes.Integration.Commands.ProjectionTypes;

/// <summary>
/// Represents the command used to create a new <see cref="ProjectionType"/>
/// </summary>
public record CreateProjectionTypeCommand
    : ICommand<IOperationResult<ProjectionType>, ProjectionType>
{

    /// <summary>
    /// Gets/sets the projection type's name
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets a short, concise description of the projection type<para></para>
    /// Supports Markdown
    /// </summary>
    public virtual string? Summary { get; set; }

    /// <summary>
    /// Gets/sets the projection type's description, if any<para></para>
    /// Supports Markdown
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets/sets the schema that defines, documents and validates the state of projections of this type
    /// </summary>
    public virtual JsonSchema Schema { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    public virtual ProjectionTriggerCollection Triggers { get; set; } = new();

    /// <summary>
    /// Gets/sets a list containing the indexes, if any, of projections of this type
    /// </summary>
    public virtual EquatableList<ProjectionIndexDefinition>? Indexes { get; set; }

    /// <summary>
    /// Gets/sets a list containing the relationships, if any, of projections of this type
    /// </summary>
    public virtual EquatableList<ProjectionRelationshipDefinition>? Relationships { get; set; }

    /// <summary>
    /// Gets/sets a key/value mapping of the tags, if any, associated to the projection type
    /// </summary>
    public virtual EquatableDictionary<string, string>? Tags { get; set; }

    /// <inheritdoc/>
    public virtual IDictionary<string, object> ContextData { get; } = new Dictionary<string, object>();

}
