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

using CloudShapes.Integration.Queries.Projections;

namespace CloudShapes.Application.Queries.Projections;

/// <summary>
/// Represents the service used to handle <see cref="GetProjectionQuery"/> instances
/// </summary>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
public class GetProjectionQueryHandler(IDbContext dbContext)
    : IQueryHandler<GetProjectionQuery, object>
{

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<object>> HandleAsync(GetProjectionQuery query, CancellationToken cancellationToken = default)
    {
        var set = DbContext.Set(query.Type);
        var projection = await set.GetAsync(query.Id, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException($"Failed to find a projection of type '{query.Type}' with id '{query.Id}'");
        return this.Ok(BsonTypeMapper.MapToDotNetValue(projection));
    }

}