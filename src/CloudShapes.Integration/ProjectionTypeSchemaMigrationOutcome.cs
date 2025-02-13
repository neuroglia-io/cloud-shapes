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

namespace CloudShapes;

/// <summary>
/// Enumerates all migration outcomes
/// </summary>
public static class ProjectionTypeSchemaMigrationOutcome
{

    /// <summary>
    /// Indicates that the projection's type schema has been updated, and that all related projections were migrated
    /// </summary>
    public const string Migrated = "migrated";
    /// <summary>
    /// Indicates that the projection's type schema has been updated, and that all related projections were validated
    /// </summary>
    public const string Validated = "validated";
    /// <summary>
    /// Indicates that the projection's type schema has been updated
    /// </summary>
    public const string Updated = "updated";

}