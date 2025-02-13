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

namespace CloudShapes.Dashboard.Pages.Projections.List;

/// <summary>
/// Represents the projection list view state
/// </summary>
public record ProjectionListState
{

    /// <summary>
    /// Gets/sets a boolean value that indicates whether data is currently being gathered
    /// </summary>
    public bool Loading { get; set; } = true;

    /// <summary>
    /// Gets/sets a list of all available <see cref="Data.Models.ProjectionType"/>s
    /// </summary>
    public EquatableList<ProjectionType> ProjectionTypes { get; set; } = [];

    /// <summary>
    /// Gets/sets the name of the type of projections to list
    /// </summary>
    public string? ProjectionTypeName { get; set; }

    /// <summary>
    /// Gets/sets the type of projections to list
    /// </summary>
    public ProjectionType? ProjectionType { get; set; }

    /// <summary>
    /// Gets/sets the options used to query projections
    /// </summary>
    public QueryOptions QueryOptions { get; set; } = new();

    /// <summary>
    /// Gets/sets the current page of projections to list, if any
    /// </summary>
    public PagedResult<IDictionary<string, object>>? Projections { get; set; }

    /// <summary>
    /// Gets/sets a list that contains the ids of all selected projections
    /// </summary>
    public EquatableList<string> SelectedProjections { get; set; } = [];

}
