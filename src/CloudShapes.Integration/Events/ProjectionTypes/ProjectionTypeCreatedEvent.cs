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

namespace CloudShapes.Integration.Events.ProjectionTypes;

/// <summary>
/// Represents the payload of the <see cref="CloudEvent"/> published whenever a new <see cref="ProjectionType"/> has been created
/// </summary>
/// <param name="Name">The <see cref="ProjectionType"/>'s name</param>
/// <param name="Summary">The <see cref="ProjectionType"/>'s summary, if any</param>
/// <param name="Description">The <see cref="ProjectionType"/>'s description, if any</param>
/// <param name="Schema">The schema that defines, documents and validates the state of projections of this type</param>
/// <param name="Triggers">A list containing the triggers responsible for creating new projections when specific CloudEvents occur</param>
/// <param name="Indexes">A list containing the indexes, if any, of projections of this type</param>
/// <param name="Relationships">A list containing the relationships, if any, of projections of this type</param>
/// <param name="Tags">A key/value mapping of the <see cref="ProjectionType"/>'s tags, if any</param>
public record ProjectionTypeCreatedEvent(string Name, string? Summary, string? Description, JsonSchema Schema, ProjectionTriggerCollection Triggers, IEnumerable<ProjectionIndexDefinition>? Indexes, IEnumerable<ProjectionRelationshipDefinition>? Relationships, IDictionary<string, string>? Tags);
