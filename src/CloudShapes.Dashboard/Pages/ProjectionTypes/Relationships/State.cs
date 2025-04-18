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

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Relationships;

/// <summary>
/// Represents the state of the relationships projection type view
/// </summary>
public record ProjectionTypeRelationshipsState
{

    /// <summary>
    /// Gets/sets the current <see cref="PageStatus"/>
    /// </summary>
    public string Status { get; set; } = PageStatus.Pending;

    /// <summary>
    /// Gets/sets the name of the <see cref="ProjectionType"/> to show
    /// </summary>
    public string ProjectionTypeName { get; set; } = null!;

    /// <summary>
    /// Gets/sets the <see cref="ProjectionType"/> before editing, if any
    /// </summary>
    public ProjectionType? ProjectionType { get; set; } = null!;

}
