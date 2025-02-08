using CloudShapes.Data.Events.ProjectionTypes;

namespace CloudShapes.Application.Commands.ProjectionTypes;

/// <summary>
/// Represents the service used to handle <see cref="CreateProjectionTypeCommand"/>s
/// </summary>
/// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
/// <param name="pluralize">The service used to pluralize terms</param>
/// <param name="cloudEventBus">The service used to observe both inbound and outbound <see cref="CloudEvent"/>s</param>
public class CreateProjectionTypeCommandHandler(IOptions<ApplicationOptions> options, IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IPluralize pluralize, ICloudEventBus cloudEventBus)
    : ICommandHandler<CreateProjectionTypeCommand, ProjectionType>
{

    /// <summary>
    /// Gets the current <see cref="ApplicationOptions"/>
    /// </summary>
    protected ApplicationOptions Options { get; } = options.Value;

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
        await ProjectionTypes.InsertOneAsync(projectionType, new InsertOneOptions(), cancellationToken).ConfigureAwait(false);
        var collectionName =  Pluralize.Pluralize(projectionType.Name);
        var collectionNames = await (await Database.ListCollectionNamesAsync(new ListCollectionNamesOptions(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        if (collectionNames.Contains(collectionName)) throw new Exception($"A ProjectionType with the specified name '{command.Name}' already exists");
        if (command.Relationships != null)
        {
            foreach(var relationship in command.Relationships)
            {
                var targetCollectionName = Pluralize.Pluralize(relationship.Target);
                if (!collectionNames.Contains(targetCollectionName)) throw new NullReferenceException($"Failed to find a ProjectionType with the specified name '{relationship.Target}'");
            }
        }
        await Database.CreateCollectionAsync(collectionName, new CreateCollectionOptions(), cancellationToken).ConfigureAwait(false);
        var collection = Database.GetCollection<BsonDocument>(collectionName);
        if (projectionType.Indexes != null)
        {
            var indexModels = new List<CreateIndexModel<BsonDocument>>();
            foreach (var index in projectionType.Indexes)
            {
                var keys = index.Text
                    ? Builders<BsonDocument>.IndexKeys.Text(index.Fields[0])
                    : index.Unique
                        ? index.Descending 
                            ? Builders<BsonDocument>.IndexKeys.Descending(index.Fields[0])
                            : Builders<BsonDocument>.IndexKeys.Ascending(index.Fields[0])
                        : index.Descending 
                            ? Builders<BsonDocument>.IndexKeys.Descending(string.Join(",", index.Fields))
                            : Builders<BsonDocument>.IndexKeys.Ascending(string.Join(",", index.Fields));
                var options = new CreateIndexOptions { Unique = index.Unique };
                indexModels.Add(new CreateIndexModel<BsonDocument>(keys, options));
            }
            await collection.Indexes.CreateManyAsync(indexModels, new CreateManyIndexesOptions(), cancellationToken).ConfigureAwait(false);
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
