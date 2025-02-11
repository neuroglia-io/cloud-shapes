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

using CloudShapes.Integration.Queries.ProjectionTypes;

namespace CloudShapes.Application.Queries.ProjectionTypes;

/// <summary>
/// Represents the service used to handle <see cref="GetProjectionTypeQuery"/> instances
/// </summary>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
public class GetProjectionTypeQueryHandler(IMongoCollection<ProjectionType> projectionTypes)
    : IQueryHandler<GetProjectionTypeQuery, ProjectionType>
{

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<ProjectionType>> HandleAsync(GetProjectionTypeQuery query, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProjectionType>.Filter.Regex("_id", new BsonRegularExpression($"^{Regex.Escape(query.Name)}$", "i"));
        var projectionType = await( await ProjectionTypes.FindAsync(filter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException($"Failed to find a projection type with the specified name '{query.Name}'");
        return this.Ok(projectionType);
    }

}