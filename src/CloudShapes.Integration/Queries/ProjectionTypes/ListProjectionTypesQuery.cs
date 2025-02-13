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

namespace CloudShapes.Integration.Queries.ProjectionTypes;

/// <summary>
/// Represents the query used to list <see cref="ProjectionType"/>s
/// </summary>
/// <param name="options">The query options</param>
public class ListProjectionTypesQuery(QueryOptions options)
    : Query<PagedResult<ProjectionType>>
{

    /// <summary>
    /// Gets the query options
    /// </summary>
    public virtual QueryOptions Options { get; } = options;

}