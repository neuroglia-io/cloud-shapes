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
using CloudShapes.Integration.Queries.Projections;

namespace CloudShapes.Application.Queries.Projections;

/// <summary>
/// Represents the service used to handle <see cref="ListProjectionsQuery"/> instances
/// </summary>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
public class ListProjectionsQueryHandler(IDbContext dbContext, IJsonSerializer jsonSerializer)
    : IQueryHandler<ListProjectionsQuery, PagedResult<object>>
{

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<PagedResult<object>>> HandleAsync(ListProjectionsQuery query, CancellationToken cancellationToken = default)
    {
        var set = DbContext.Set(query.Type);
        var filter = query.Options.BuildFilter<BsonDocument>(set.Type);
        var totalCount = await set.CountAsync(filter, cancellationToken).ConfigureAwait(false);
        var cursor = await set.FindAsync(filter, cancellationToken).ConfigureAwait(false);
        var results = cursor.ToAsyncEnumerable(cancellationToken);
        if (query.Options.Skip.HasValue) results = results.Skip(query.Options.Skip.Value);
        if (query.Options.Limit.HasValue) results = results.Take(query.Options.Limit.Value);
        var items = await results.Select(BsonTypeMapper.MapToDotNetValue).ToListAsync(cancellationToken).ConfigureAwait(false);
        var pageResult = new PagedResult<object>(items, totalCount, query.Options.Limit, query.Options.Limit.HasValue ? ((query.Options.Skip ?? 0) / query.Options.Limit.Value) + 1 : null);
        return this.Ok(pageResult);
    }

}
