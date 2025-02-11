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

using CloudShapes.Integration;
using CloudShapes.Integration.Commands.ProjectionTypes;

namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes Projection Types API
/// </summary>
public interface IProjectionTypesApiClient
{

    /// <summary>
    /// Creates a new <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created <see cref="ProjectionType"/></returns>
    Task<ProjectionType> CreateAsync(CreateProjectionTypeCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="ProjectionType"/> with the specified name
    /// </summary>
    /// <param name="name">The name of the <see cref="ProjectionType"/> to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ProjectionType"/> with the specified name</returns>
    Task<ProjectionType> GetAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists <see cref="ProjectionType"/>s
    /// </summary>
    /// <param name="queryOptions">The query's options</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="PagedResult{T}"/> that wraps filtered <see cref="ProjectionType"/>s</returns>
    Task<PagedResult<ProjectionType>> ListAsync(QueryOptions? queryOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the <see cref="ProjectionType"/> with the specified name
    /// </summary>
    /// <param name="name">The name of the <see cref="ProjectionType"/> to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);

}
