using CloudShapes.Data.Events.ProjectionTypes;

namespace CloudShapes.Application.Commands.ProjectionTypes;

/// <summary>
/// Represents the service used to handle <see cref="DeleteProjectionTypeCommand"/>s
/// </summary>
/// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="pluralize">The service used to pluralize terms</param>
/// <param name="cloudEventBus">The service used to observe both inbound and outbound <see cref="CloudEvent"/>s</param>
public class DeleteProjectionTypeCommandHandler(IOptions<ApplicationOptions> options, IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext, IPluralize pluralize, ICloudEventBus cloudEventBus)
    : ICommandHandler<DeleteProjectionTypeCommand>
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
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to pluralize terms
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <summary>
    /// Gets the service used to observe both inbound and outbound <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventBus CloudEventBus { get; } = cloudEventBus;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult> HandleAsync(DeleteProjectionTypeCommand command, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProjectionType>.Filter.Regex("_id", new BsonRegularExpression($"^{Regex.Escape(command.Name)}$", "i"));
        var projectionType = await (await ProjectionTypes.FindAsync(filter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException($"Failed to find a projection type with the specified name '{command.Name}'");
        var projections = DbContext.Set(projectionType);
        await foreach (var projection in projections.ToListAsync(cancellationToken)) await projections.DeleteAsync(projection.GetId()!, cancellationToken).ConfigureAwait(false);
        await ProjectionTypes.DeleteOneAsync(Builders<ProjectionType>.Filter.Eq("_id", BsonValue.Create(projectionType.Name)), new DeleteOptions(), cancellationToken).ConfigureAwait(false);
        await Database.DropCollectionAsync(Pluralize.Pluralize(projectionType.Name), cancellationToken).ConfigureAwait(false);
        CloudEventBus.OutputStream.OnNext(new CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Time = DateTimeOffset.Now,
            Source = Options.Events.Source,
            Type = CloudShapes.CloudEvents.ProjectionTypes.Created.V1.Type,
            Subject = projectionType.Name,
            DataContentType = MediaTypeNames.Application.Json,
            Data = new ProjectionTypeDeletedEvent(projectionType.Name)
        });
        return this.Ok();
    }

}