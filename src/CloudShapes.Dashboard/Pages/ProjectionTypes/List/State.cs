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

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.List;

/// <summary>
/// Represents the projection type list view state
/// </summary>
public record ProjectionTypeListState
{

    /// <summary>
    /// Gets/sets a boolean value that indicates whether data is currently being gathered
    /// </summary>
    public bool Loading { get; set; } = true;

    /// <summary>
    /// Gets/sets maximum number of projection types to return.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets/sets the number of projection types to skip, used for pagination.
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// Gets/sets the term to search for, if any
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets/sets the property to order items by, if any
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not to order projection types in a descending fashion. Ignored if <see cref="OrderBy"/> has not been set
    /// </summary>
    public bool Descending { get; set; } = false;

    /// <summary>
    /// Gets/sets a dictionary of filters where the key is the field name and the value is the expected value.
    /// </summary>
    public virtual EquatableDictionary<string, string>? Filters { get; set; }

    /// <summary>
    /// Gets/sets the current page of <see cref="ProjectionType"/>s to list
    /// </summary>
    public PagedResult<ProjectionType>? ProjectionTypes { get; set; }

    /// <summary>
    /// Gets/sets a list that contains the ids of all selected projections
    /// </summary>
    public EquatableList<string> SelectedProjectionTypes { get; set; } = [];

}
