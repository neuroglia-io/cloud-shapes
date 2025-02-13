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

namespace CloudShapes.Integration.Models;

/// <summary>
/// Represents the result of the update of a projection type's schema
/// </summary>
public record ProjectionTypeSchemaMigrationResult
{

    /// <summary>
    /// Gets/sets the migration's outcome<para>See <see cref="ProjectionTypeSchemaMigrationOutcome"/></para>
    /// </summary>
    public virtual string Outcome { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing warnings, fi any, that have been raised during migration
    /// </summary>
    public virtual EquatableList<string>? Warnings { get; set; }

    /// <summary>
    /// Gets/sets a list that contains the validation errors, if any, that have occurred during projection migration
    /// </summary>
    public virtual EquatableList<ProjectionValidationResult>? ValidationErrors { get; set; }

    /// <summary>
    /// Gets/sets the amount of projections processed during migration
    /// </summary>
    public virtual long ProcessedProjections { get; set; }

}
