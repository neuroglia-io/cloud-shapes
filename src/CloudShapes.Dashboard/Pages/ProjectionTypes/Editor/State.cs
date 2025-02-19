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

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Editor;

/// <summary>
/// Represents the state of the create projection type view
/// </summary>
public record ProjectionTypeEditorState
{

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the view is loading
    /// </summary>
    public bool Loading { get; set; } = false;

    /// <summary>
    /// Gets/sets the name of the <see cref="ProjectionType"/> to edit, if any
    /// </summary>
    public string? ProjectionTypeName { get; set; } = null;

    /// <summary>
    /// Gets/sets the <see cref="ProjectionType"/> before editing, if any
    /// </summary>
    public ProjectionType? OriginalProjectionType = null;

    /// <summary>
    /// Gets/sets the projection type's name used in the editor
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Gets/sets a short, concise description of the projection type<para></para>
    /// Supports Markdown
    /// </summary>
    public string? Summary { get; set; } = null;

    /// <summary>
    /// Gets/sets the projection type's description, if any<para></para>
    /// Supports Markdown
    /// </summary>
    public string? Description { get; set; } = null;

    /// <summary>
    /// Gets/sets the schema that defines, documents and validates the state of projections of this type
    /// </summary>
    public JsonSchema? Schema { get; set; } = null;

    /// <summary>
    /// Gets/sets the schema that defines, documents and validates the state of projections of this type
    /// </summary>
    public string? SerializedSchema { get; set; } = null;

    /// <summary>
    /// Gets/sets the type of the migration patch
    /// </summary>
    public string? MigrationPatchType { get; set; } = null;

    /// <summary>
    /// Gets/sets the migration patch that transforms the state of projections of this type from the previous schema to the current schema
    /// </summary>
    public string? SerializedMigration { get; set; } = null;

    /// <summary>
    /// Gets/sets a boolean indicating whether or not to validate the projections of the migrated type.
    /// </summary>
    public bool ValidateMigration { get; set; } = false;

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    public ProjectionTriggerCollection Triggers { get; set; } = new();

    /// <summary>
    /// Gets/sets a list containing the indexes, if any, of projections of this type
    /// </summary>
    public EquatableList<ProjectionIndexDefinition> Indexes { get; set; } = [];

    /// <summary>
    /// Gets/sets a list containing the relationships, if any, of projections of this type
    /// </summary>
    public EquatableList<ProjectionRelationshipDefinition> Relationships { get; set; } = [];

    /// <summary>
    /// Gets/sets a key/value mapping of the tags, if any, associated to the projection type
    /// </summary>
    public EquatableDictionary<string, string> Tags { get; set; } = [];

    /// <summary>
    /// Gets/sets a list containing the errors that have occurred during the validation or the creation of the projection type
    /// </summary>
    public EquatableList<string> Errors { get; set; } = [];

}
