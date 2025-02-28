﻿// Copyright © 2025-Present The Cloud Shapes Authors
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

namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage projections of a specific type
/// </summary>
public interface IRepository
{

    /// <summary>
    /// Gets the type of projections managed by the <see cref="IRepository"/>
    /// </summary>
    ProjectionType Type { get; }

    /// <summary>
    /// Determines whether or not the specified projection exists
    /// </summary>
    /// <param name="id">The id of the projection to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the specified projection exists</returns>
    Task<bool> ContainsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the projection with the specified id
    /// </summary>
    /// <param name="id">The id of the projection to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The projection with the specified id</returns>
    Task<BsonDocument?> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds projections that match the specified filter
    /// </summary>
    /// <param name="filter">The filter to match</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncCursor{TDocument}"/> used to enumerate matches</returns>
    Task<IAsyncCursor<BsonDocument>> FindAsync(FilterDefinition<BsonDocument> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the specified projection
    /// </summary>
    /// <param name="projection">The projection to add</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly added <see cref="BsonDocument"/></returns>
    Task<BsonDocument> AddAsync(BsonDocument projection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified projection
    /// </summary>
    /// <param name="projection">The updated projection</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The updated <see cref="BsonDocument"/></returns>
    Task<BsonDocument> UpdateAsync(BsonDocument projection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Patches the specified projection
    /// </summary>
    /// <param name="id">The id of the projection to patch</param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The patched projection</returns>
    Task<BsonDocument> PatchAsync(string id, Patch patch, CancellationToken cancellationToken = default);

    /// <summary>
    /// Patches the specified projection
    /// </summary>
    /// <param name="projection">The projection to patch</param>
    /// <param name="patch">The patch to apply</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The patched projection</returns>
    Task<BsonDocument> PatchAsync(BsonDocument projection, Patch patch, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified projection
    /// </summary>
    /// <param name="id">The id of the projection to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of projections in the collection
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The count of projections in the collection</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of projections in the collection that match the specified filter
    /// </summary>
    /// <param name="filter">The filter definition used to match documents</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The count of projections in the collection that match the specified filter</returns>
    Task<long> CountAsync(FilterDefinition<BsonDocument> filter, CancellationToken cancellationToken= default);

    /// <summary>
    /// Lists all projections contained by the repository
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to asynchronously enumerate all projections contained by the repository</returns>
    IAsyncEnumerable<BsonDocument> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a queryable source of projections
    /// </summary>
    /// <returns>A new <see cref="IQueryable{T}"/></returns>
    IQueryable<BsonDocument> AsQueryable();

}
