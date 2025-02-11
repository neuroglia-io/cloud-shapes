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
/// Represents the service used to handle <see cref="CreateProjectionCommand"/>s
/// </summary>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
public class CreateProjectionCommandHandler(IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext, IJsonSerializer jsonSerializer)
    : ICommandHandler<CreateProjectionCommand, object>
{

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the  current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<object>> HandleAsync(CreateProjectionCommand command, CancellationToken cancellationToken = default)
    {
        var type = await (await ProjectionTypes.FindAsync(Builders<ProjectionType>.Filter.Eq("_id", command.Type), new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, $"Failed to find a projection type with the specified name '{command.Type}'"));
        var set = DbContext.Set(type);
        var json = JsonSerializer.SerializeToText(command.State);
        var document = BsonDocument.Parse(json);
        document["_id"] = command.Id;
        try
        {
            await set.AddAsync(document, cancellationToken).ConfigureAwait(false);
        }
        catch (MongoWriteException ex) when(ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new ProblemDetailsException(new(Problems.Types.KeyAlreadyExists, Problems.Titles.KeyAlreadyExists, Problems.Statuses.Unprocessable, $"A projection of type '{command.Type}' with the specified id '{command.Id}' already exists"));
        }
        return this.Ok(BsonTypeMapper.MapToDotNetValue(document));
    }

}
