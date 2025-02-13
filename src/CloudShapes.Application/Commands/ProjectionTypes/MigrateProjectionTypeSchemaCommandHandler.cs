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

using CloudShapes.Integration.Commands.ProjectionTypes;

namespace CloudShapes.Application.Commands.ProjectionTypes;

/// <summary>
/// Represents the service used to handle <see cref="MigrateProjectionTypeSchemaCommand"/>s
/// </summary>
/// <param name="projectionTypes">The service used to manage <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="schemaValidator">The service used to validate data against <see cref="JsonSchema"/>s</param>
public class MigrateProjectionTypeSchemaCommandHandler(IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext, ISchemaValidator schemaValidator)
    : ICommandHandler<MigrateProjectionTypeSchemaCommand, ProjectionTypeSchemaMigrationResult>
{

    /// <summary>
    /// Gets the service used to manage <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to validate data against <see cref="JsonSchema"/>s
    /// </summary>
    protected ISchemaValidator SchemaValidator { get; } = schemaValidator;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<ProjectionTypeSchemaMigrationResult>> HandleAsync(MigrateProjectionTypeSchemaCommand command, CancellationToken cancellationToken = default)
    {
        var projectionType = await (await ProjectionTypes.FindAsync(Builders<ProjectionType>.Filter.Regex("_id", new BsonRegularExpression($"^{Regex.Escape(command.Name)}$", "i")), new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, StringFormatter.Format(Problems.Details.ProjectionTypeNotFound, command.Name)));
        projectionType.Schema = command.Schema;
        await ProjectionTypes.ReplaceOneAsync(Builders<ProjectionType>.Filter.Eq("_id", projectionType.Name), projectionType, new ReplaceOptions(), cancellationToken).ConfigureAwait(false);
        var migrationResult = new ProjectionTypeSchemaMigrationResult()
        {
            Outcome = command.Migration != null ? ProjectionTypeSchemaMigrationOutcome.Migrated : command.Validate ? ProjectionTypeSchemaMigrationOutcome.Validated : ProjectionTypeSchemaMigrationOutcome.Updated
        };
        if (command.Migration != null || command.Validate)
        {
            var dbSet = DbContext.Set(projectionType);
            await foreach (var projection in dbSet.ToListAsync(cancellationToken))
            {
                var projectionId = projection["_id"].ToString()!;
                var projectionState = BsonTypeMapper.MapToDotNetValue(projection);
                if (command.Migration == null)
                {
                    var validationResult = await SchemaValidator.ValidateAsync(projectionState, projectionType.Schema, cancellationToken).ConfigureAwait(false);
                    if (!validationResult.IsSuccess())
                    {
                        migrationResult.ValidationErrors ??= [];
                        migrationResult.ValidationErrors.Add(new(projectionId, [.. validationResult.Errors!.GroupBy(e => e.Instance?.OriginalString!).ToDictionary(g => g.Key, g => g.Select(e => e.Detail).ToArray())!]));
                    }
                }
                else
                {
                    try
                    {
                        await dbSet.PatchAsync(projection, command.Migration, cancellationToken).ConfigureAwait(false);
                    }
                    catch (ProblemDetailsException ex) when (ex.Problem.Type == Problems.Types.ValidationFailed)
                    {
                        migrationResult.ValidationErrors ??= [];
                        migrationResult.ValidationErrors.Add(new(projectionId, [..ex.Problem.Errors!]));
                    }
                }
                migrationResult.ProcessedProjections++;
            }
        }
        return this.Ok(migrationResult);
    }

}
