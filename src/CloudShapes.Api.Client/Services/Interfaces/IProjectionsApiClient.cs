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

using CloudShapes.Integration.Commands.Projections;

namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes Projections API
/// </summary>
public interface IProjectionsApiClient
{

    /// <summary>
    /// Creates a new projection
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created projection</returns>
    Task<object> CreateAsync(CreateProjectionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the specified projection
    /// </summary>
    /// <param name="type">The type of the projection to get</param>
    /// <param name="id">The id of the projection to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified projection</returns>
    Task<object> GetAsync(string type, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists projections of the specified type
    /// </summary>
    /// <param name="type">The type of projections to list</param>
    /// <param name="queryOptions">The query's options</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="PagedResult{T}"/> that wraps filtered projections</returns>
    Task<PagedResult<IDictionary<string, object>>> ListAsync(string type, QueryOptions? queryOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing projection
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created projection</returns>
    Task<object> UpdateAsync(UpdateProjectionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Patches an existing projection
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created projection</returns>
    Task<object> PatchAsync(PatchProjectionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified projection
    /// </summary>
    /// <param name="type">The type of projection to delete</param>
    /// <param name="id">The id of the projection to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(string type, string id, CancellationToken cancellationToken = default);

}