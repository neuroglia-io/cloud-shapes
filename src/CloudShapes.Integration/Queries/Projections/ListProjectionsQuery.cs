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

namespace CloudShapes.Integration.Queries.Projections;

/// <summary>
/// Represents the query used to list <see cref="ProjectionType"/>s
/// </summary>
/// <param name="type">The name of the type of projections to list</param>
/// <param name="options">The query options</param>
public class ListProjectionsQuery(string type, QueryOptions options)
    : Query<PagedResult<object>>
{

    /// <summary>
    /// Gets the name of the type of projections to list
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets the query options
    /// </summary>
    public virtual QueryOptions Options { get; } = options;

}