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
using CloudShapes.Integration.Events.ProjectionTypes;

namespace CloudShapes.Application.Commands.ProjectionTypes;

/// <summary>
/// Represents the service used to handle <see cref="CreateProjectionTypeCommand"/>s
/// </summary>
/// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
/// <param name="mediator">The service used to mediate calls</param>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
/// <param name="pluralize">The service used to pluralize terms</param>
/// <param name="cloudEventBus">The service used to observe both inbound and outbound <see cref="CloudEvent"/>s</param>
public class CreateProjectionTypeCommandHandler(IOptions<ApplicationOptions> options, IMediator mediator, IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IPluralize pluralize, ICloudEventBus cloudEventBus)
    : ICommandHandler<CreateProjectionTypeCommand, ProjectionType>
{

    /// <summary>
    /// Gets the current <see cref="ApplicationOptions"/>
    /// </summary>
    protected ApplicationOptions Options { get; } = options.Value;

    /// <summary>
    /// Gets the service used to mediate calls
    /// </summary>
    protected IMediator Mediator { get; } = mediator;

    /// <summary>
    /// Gets the current <see cref="IMongoDatabase"/>
    /// </summary>
    protected IMongoDatabase Database { get; } = database;

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the service used to pluralize terms
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <summary>
    /// Gets the service used to observe both inbound and outbound <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventBus CloudEventBus { get; } = cloudEventBus;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<ProjectionType>> HandleAsync(CreateProjectionTypeCommand command, CancellationToken cancellationToken = default)
    {
        var projectionType = new ProjectionType(command.Name, command.Schema, command.Triggers, command.Indexes, command.Relationships, command.Summary, command.Description, command.Tags);
        try
        {
            await ProjectionTypes.InsertOneAsync(projectionType, new InsertOneOptions(), cancellationToken).ConfigureAwait(false);
        }
        catch (MongoWriteException ex) when(ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new ProblemDetailsException(new(Problems.Types.KeyAlreadyExists, Problems.Titles.KeyAlreadyExists, Problems.Statuses.Unprocessable, $"A projection type with the specified name '{command.Name}' already exists"));
        }
        var collectionName =  Pluralize.Pluralize(projectionType.Name);
        var collectionNames = await (await Database.ListCollectionNamesAsync(new ListCollectionNamesOptions(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        if (collectionNames.Contains(collectionName)) throw new ProblemDetailsException(new(Problems.Types.KeyAlreadyExists, Problems.Titles.KeyAlreadyExists, Problems.Statuses.Unprocessable, $"A projection type with the specified name '{command.Name}' already exists"));
        if (command.Relationships != null)
        {
            foreach(var relationship in command.Relationships)
            {
                var targetCollectionName = Pluralize.Pluralize(relationship.Target);
                if (!collectionNames.Contains(targetCollectionName)) throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, $"Failed to find a projection type with the specified name '{relationship.Target}'"));
            }
        }
        await Database.CreateCollectionAsync(collectionName, new CreateCollectionOptions(), cancellationToken).ConfigureAwait(false);
        var collection = Database.GetCollection<BsonDocument>(collectionName);
        if (projectionType.Indexes != null)
        {
            foreach (var index in projectionType.Indexes)
            {
                var keys = index.Text
                    ? Builders<BsonDocument>.IndexKeys.Text(index.Properties[0])
                    : index.Unique
                        ? index.Descending
                            ? Builders<BsonDocument>.IndexKeys.Descending(index.Properties[0])
                            : Builders<BsonDocument>.IndexKeys.Ascending(index.Properties[0])
                        : index.Descending
                            ? Builders<BsonDocument>.IndexKeys.Descending(string.Join(",", index.Properties))
                            : Builders<BsonDocument>.IndexKeys.Ascending(string.Join(",", index.Properties));
                var options = new CreateIndexOptions { Unique = index.Unique };
                try
                {
                    await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(keys, options), new CreateOneIndexOptions(), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await Mediator.ExecuteAndUnwrapAsync(new DeleteProjectionTypeCommand(projectionType.Name), cancellationToken).ConfigureAwait(false);
                    throw new ProblemDetailsException(new ProblemDetails(Problems.Types.IndexCreationFailed, Problems.Titles.IndexCreationFailed, Problems.Statuses.Unprocessable, $"Failed to create the index with name '{index.Name}': {ex.Message}"));
                }
            }
        }
        CloudEventBus.OutputStream.OnNext(new CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Time = DateTimeOffset.Now,
            Source = Options.Events.Source,
            Type = CloudShapes.CloudEvents.ProjectionTypes.Created.V1.Type,
            Subject = projectionType.Name,
            DataContentType = MediaTypeNames.Application.Json,
            Data = new ProjectionTypeCreatedEvent(projectionType.Name, projectionType.Summary, projectionType.Description, projectionType.Schema, projectionType.Triggers, projectionType.Indexes, projectionType.Relationships, projectionType.Tags)
        });
        return this.Ok(projectionType);
    }

}
