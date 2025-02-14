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

namespace CloudShapes.Application.Commands.Projections;

/// <summary>
/// Represents the service used to handle <see cref="DeleteProjectionCommand"/>s
/// </summary>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="pluralize">The service used to pluralize words</param>
public class DeleteProjectionCommandHandler(IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext, IPluralize pluralize)
    : ICommandHandler<DeleteProjectionCommand>
{

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to pluralize words
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult> HandleAsync(DeleteProjectionCommand command, CancellationToken cancellationToken = default)
    {
        var typeName = command.Type;
        if (Pluralize.IsPlural(typeName)) typeName = Pluralize.Singularize(typeName);
        var type = await (await ProjectionTypes.FindAsync(Builders<ProjectionType>.Filter.Regex("_id", new BsonRegularExpression(typeName, "i")), new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
           ?? throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, StringFormatter.Format(Problems.Details.ProjectionTypeNotFound, typeName)));
        var set = DbContext.Set(type);
        if (!await set.ContainsAsync(command.Id, cancellationToken).ConfigureAwait(false)) throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, StringFormatter.Format(Problems.Details.ProjectionNotFound, typeName, command.Id)));
        await set.DeleteAsync(command.Id, cancellationToken).ConfigureAwait(false);
        return this.Ok();
    }

}
