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

using CloudShapes.Integration.Models;

namespace CloudShapes.Integration.Commands.ProjectionTypes;

/// <summary>
/// Represents the command used to update and migrate the schema of a projection type
/// </summary>
public class MigrateProjectionTypeSchemaCommand
    : Command<ProjectionTypeSchemaMigrationResult>
{

    /// <summary>
    /// Gets/sets the name of the projection type to update
    /// </summary>
    [Required, MinLength(1)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the projection type's updated schema
    /// </summary>
    [Required]
    public virtual JsonSchema Schema { get; set; } = null!;

    /// <summary>
    /// Gets/sets the patch to apply to all projections of the updated type to migrate their state to the new schema
    /// </summary>
    public virtual Patch? Migration { get; set; }

    /// <summary>
    /// Gets/sets a boolean indicating whether or not to validate the projections of the updated type. Ignored if `migration` has been set
    /// </summary>
    public virtual bool Validate { get; set; }

}
