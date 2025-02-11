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
using CloudShapes.Integration.Queries.ProjectionTypes;

namespace CloudShapes.Application.Queries.ProjectionTypes;

/// <summary>
/// Represents the service used to handle <see cref="ListProjectionTypesQuery"/> instances
/// </summary>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
public class ListProjectionTypesQueryHandler(IMongoCollection<ProjectionType> projectionTypes)
    : IQueryHandler<ListProjectionTypesQuery, PagedResult<ProjectionType>>
{

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<PagedResult<ProjectionType>>> HandleAsync(ListProjectionTypesQuery query, CancellationToken cancellationToken = default)
    {
        var filter = query.Options.BuildFilter<ProjectionType>();
        var totalCount = await ProjectionTypes.CountDocumentsAsync(filter, new CountOptions(), cancellationToken).ConfigureAwait(false);
        var cursor = await ProjectionTypes.FindAsync(filter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false);
        var results = cursor.ToAsyncEnumerable(cancellationToken);
        if (query.Options.Skip.HasValue) results = results.Skip(query.Options.Skip.Value);
        if (query.Options.Limit.HasValue) results = results.Take(query.Options.Limit.Value);
        var items = await results.ToListAsync(cancellationToken).ConfigureAwait(false);
        var pageResult = new PagedResult<ProjectionType>(items, totalCount, query.Options.Limit, query.Options.Limit.HasValue ? ((query.Options.Skip ?? 0) / query.Options.Limit.Value) + 1 : null);
        return this.Ok(pageResult);
    }

}
