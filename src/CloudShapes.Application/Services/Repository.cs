namespace CloudShapes.Application.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IRepository"/> interface
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
/// <param name="projections">The <see cref="IMongoCollection{TDocument}"/> used to store projections managed by the <see cref="IRepository"/></param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="cloudEventBus">The service used to observe both inbound and outbound <see cref="CloudEvent"/>s</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="type">The type of projections managed by the <see cref="IRepository"/></param>
public class Repository(ILogger<Repository> logger, IOptions<ApplicationOptions> options, IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, 
    IMongoCollection<BsonDocument> projections, IDbContext dbContext, ICloudEventBus cloudEventBus, IJsonSerializer jsonSerializer, ProjectionType type)
    : IRepository
{

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

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
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to store projections managed by the <see cref="IRepository"/>
    /// </summary>
    protected IMongoCollection<BsonDocument> Projections { get; } = projections;

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to observe both inbound and outbound <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventBus CloudEventBus { get; } = cloudEventBus;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <summary>
    /// Gets the type of projections managed by the <see cref="IRepository"/>
    /// </summary>
    public ProjectionType Type { get; } = type;

    /// <inheritdoc/>
    public virtual async Task<bool> ContainsAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(id));
        return await (await Projections.FindAsync(filter, new FindOptions<BsonDocument, BsonDocument>(), cancellationToken).ConfigureAwait(false)).AnyAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<BsonDocument?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(id));
        return await (await Projections.FindAsync(filter, new FindOptions<BsonDocument, BsonDocument>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<IAsyncCursor<BsonDocument>> FindAsync(FilterDefinition<BsonDocument> filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Projections.FindAsync(filter, new FindOptions<BsonDocument, BsonDocument>(), cancellationToken);
        }
        catch (MongoCommandException ex) when (ex.CodeName == "IndexNotFound")
        {
            return new EmptyAsyncCursor<BsonDocument>();
        }
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(BsonDocument projection, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projection);
        var projectionId = projection["_id"].ToString()!;
        projection = projection.InsertMetadata(new DocumentMetadata());
        if (Type.Relationships != null)
        {
            foreach (var relationship in Type.Relationships.Where(r => r.Type == ProjectionRelationshipType.OneToOne))
            {
                var path = relationship.Path;
                var key = projection.Find(relationship.Key);
                if (key == null) continue;
                var target = await DbContext.Set(relationship.Target).GetAsync(key.ToString()!, cancellationToken).ConfigureAwait(false);
                if (target == null) continue;
                if (relationship.IsForeignKeyPathAligned) target["_id"] = BsonValue.Create(key);
                else target.Remove("_id");
                projection.Replace(path, target);
            }
        }
        await Projections.InsertOneAsync(projection, new InsertOneOptions(), cancellationToken).ConfigureAwait(false);
        CloudEventBus.OutputStream.OnNext(new CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Time = DateTimeOffset.Now,
            Source = Options.Events.Source,
            Type = CloudEvents.Projections.Created.V1.Type,
            Subject = projectionId,
            DataContentType = MediaTypeNames.Application.Json,
            Data = new ProjectionCreatedEvent(projectionId, Type.Name, projection.GetState(JsonSerializer))
        });
        Type.Metadata.ProjectionCount++;
        await ProjectionTypes.UpdateOneAsync(Builders<ProjectionType>.Filter.Eq("_id", BsonValue.Create(Type.Name)), Builders<ProjectionType>.Update.Set($"{nameof(ProjectionType.Metadata).ToCamelCase()}.{nameof(ProjectionTypeMetadata.ProjectionCount).ToCamelCase()}", BsonValue.Create(Type.Metadata.ProjectionCount)), new UpdateOptions(), cancellationToken).ConfigureAwait(false);
        var relatedProjectionTypesFilterBuilder = Builders<ProjectionType>.Filter;
        var relatedProjectionTypesFilter = relatedProjectionTypesFilterBuilder.ElemMatch(nameof(ProjectionType.Relationships).ToCamelCase(), relatedProjectionTypesFilterBuilder.And(
            relatedProjectionTypesFilterBuilder.Eq(nameof(ProjectionRelationshipDefinition.Type).ToCamelCase(), ProjectionRelationshipType.OneToMany),
            relatedProjectionTypesFilterBuilder.Eq(nameof(ProjectionRelationshipDefinition.Target).ToCamelCase(), Type.Name)));
        var relatedProjectionTypes = await (await ProjectionTypes.FindAsync(relatedProjectionTypesFilter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        foreach (var relatedProjectionType in relatedProjectionTypes)
        {
            var relatedProjections = DbContext.Set(relatedProjectionType);
            foreach (var relationship in relatedProjectionType.Relationships!.Where(r => r.Type == ProjectionRelationshipType.OneToMany && r.Target == Type.Name))
            {
                var foreignKey = projection.Find(relationship.Key);
                if (foreignKey == null || foreignKey == BsonNull.Value) continue;
                var relatedProjectionsFilterBuilder = Builders<BsonDocument>.Filter;
                var relatedProjectionsFilter = relatedProjectionsFilterBuilder.Eq("_id", foreignKey);
                var relatedProjection = await (await relatedProjections.FindAsync(relatedProjectionsFilter, cancellationToken)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (relatedProjection == null)
                {
                    Logger.LogWarning("Failed to find a projection of type '{type}' with the specified id '{id}'", relatedProjectionType.Name, foreignKey.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
                    continue;
                }
                relatedProjection.AddTo(relationship.Path, projection);
                await relatedProjections.UpdateAsync(relatedProjection, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public virtual async Task UpdateAsync(BsonDocument projection, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projection);
        var metadata = BsonSerializer.Deserialize<DocumentMetadata>(projection[DocumentMetadata.PropertyName].AsBsonDocument);
        metadata.Update();
        projection = projection.InsertMetadata(metadata);
        var projectionId = BsonValue.Create(projection.GetId());
        var result = await Projections.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", projectionId), projection, new ReplaceOptions(), cancellationToken).ConfigureAwait(false);
        CloudEventBus.OutputStream.OnNext(new CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Time = DateTimeOffset.Now,
            Source = Options.Events.Source,
            Type = CloudEvents.Projections.Updated.V1.Type,
            Subject = projectionId.ToString(),
            DataContentType = MediaTypeNames.Application.Json,
            Data = new ProjectionUpdatedEvent(projectionId.ToString()!, Type.Name, projection.GetState(JsonSerializer))
        });
        if (result.MatchedCount < 1) return;
        var relatedProjectionTypesFilterBuilder = Builders<ProjectionType>.Filter;
        var relatedProjectionTypesFilter = relatedProjectionTypesFilterBuilder.ElemMatch(nameof(ProjectionType.Relationships).ToCamelCase(), relatedProjectionTypesFilterBuilder.Eq(nameof(ProjectionRelationshipDefinition.Target).ToCamelCase(), Type.Name));
        var relatedProjectionTypes = await (await ProjectionTypes.FindAsync(relatedProjectionTypesFilter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        foreach (var relatedProjectionType in relatedProjectionTypes)
        {
            var relatedProjections = DbContext.Set(relatedProjectionType);
            foreach (var relationship in relatedProjectionType.Relationships!.Where(r => r.Target == Type.Name))
            {
                switch (relationship.Type)
                {
                    case ProjectionRelationshipType.OneToOne:
                        var relatedProjectionsFilterBuilder = Builders<BsonDocument>.Filter;
                        var foreignKeyPath = relationship.IsForeignKeyPathAligned ? $"{relationship.Key}._id" : relationship.Key;
                        var relatedProjectionsFilter = relatedProjectionsFilterBuilder.Eq(foreignKeyPath, projectionId);
                        foreach (var relation in await (await relatedProjections.FindAsync(relatedProjectionsFilter, cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false))
                        {
                            relation.Replace(relationship.Path, projection);
                            await relatedProjections.UpdateAsync(relation, cancellationToken).ConfigureAwait(false);
                        }
                        break;
                    case ProjectionRelationshipType.OneToMany:
                        var foreignKey = projection.Find(relationship.Key);
                        if (foreignKey == null || foreignKey == BsonNull.Value) continue;
                        relatedProjectionsFilterBuilder = Builders<BsonDocument>.Filter;
                        relatedProjectionsFilter = relatedProjectionsFilterBuilder.Eq("_id", foreignKey);
                        var relatedProjection = await (await relatedProjections.FindAsync(relatedProjectionsFilter, cancellationToken)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                        if (relatedProjection == null)
                        {
                            Logger.LogWarning("Failed to find a projection of type '{type}' with the specified id '{id}'", relatedProjectionType.Name, foreignKey.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
                            continue;
                        }
                        relatedProjection.ReplaceInArray(relationship.Path, projection);
                        await relatedProjections.UpdateAsync(relatedProjection, cancellationToken).ConfigureAwait(false);
                        break;
                }
            }
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var projection = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        if (projection == null)
        {
            Logger.LogWarning("Failed to find a projection of type '{type}' with the specified id '{id}'", Type.Name, id);
            return;
        }
        await Projections.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(id)), new DeleteOptions(), cancellationToken).ConfigureAwait(false);
        CloudEventBus.OutputStream.OnNext(new CloudEvent()
        {
            Id = Guid.NewGuid().ToString("N"),
            Time = DateTimeOffset.Now,
            Source = Options.Events.Source,
            Type = CloudEvents.Projections.Deleted.V1.Type,
            Subject = id,
            DataContentType = MediaTypeNames.Application.Json,
            Data = new ProjectionDeletedEvent(id, Type.Name),
        });
        Type.Metadata.ProjectionCount--;
        await ProjectionTypes.UpdateOneAsync(Builders<ProjectionType>.Filter.Eq("_id", BsonValue.Create(Type.Name)), Builders<ProjectionType>.Update.Set($"{nameof(ProjectionType.Metadata).ToCamelCase()}.{nameof(ProjectionTypeMetadata.ProjectionCount).ToCamelCase()}", BsonValue.Create(Type.Metadata.ProjectionCount)), new UpdateOptions(), cancellationToken).ConfigureAwait(false);
        var relatedProjectionTypesFilterBuilder = Builders<ProjectionType>.Filter;
        var relatedProjectionTypesFilter = relatedProjectionTypesFilterBuilder.ElemMatch(nameof(ProjectionType.Relationships).ToCamelCase(), relatedProjectionTypesFilterBuilder.Eq(nameof(ProjectionRelationshipDefinition.Target).ToCamelCase(), Type.Name));
        var relatedProjectionTypes = await (await ProjectionTypes.FindAsync(relatedProjectionTypesFilter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        foreach (var relatedProjectionType in relatedProjectionTypes)
        {
            var relatedProjections = DbContext.Set(relatedProjectionType);
            foreach (var relationship in relatedProjectionType.Relationships!.Where(r => r.Target == Type.Name))
            {
                switch (relationship.Type)
                {
                    case ProjectionRelationshipType.OneToOne:
                        var relatedProjectionsFilterBuilder = Builders<BsonDocument>.Filter;
                        var foreignKeyPath = relationship.IsForeignKeyPathAligned ? $"{relationship.Key}._id" : relationship.Key;
                        var relatedProjectionsFilter = relatedProjectionsFilterBuilder.Eq(foreignKeyPath, BsonValue.Create(id));
                        foreach (var relation in await (await relatedProjections.FindAsync(relatedProjectionsFilter, cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false))
                        {
                            relation.RemoveAt(relationship.Path);
                            await relatedProjections.UpdateAsync(relation, cancellationToken).ConfigureAwait(false);
                        }
                        break;
                    case ProjectionRelationshipType.OneToMany:
                        var foreignKey = projection.Find(relationship.Key);
                        if (foreignKey == null || foreignKey == BsonNull.Value) continue;
                        relatedProjectionsFilterBuilder = Builders<BsonDocument>.Filter;
                        relatedProjectionsFilter = relatedProjectionsFilterBuilder.Eq("_id", foreignKey);
                        var relatedProjection = await (await relatedProjections.FindAsync(relatedProjectionsFilter, cancellationToken)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                        if (relatedProjection == null)
                        {
                            Logger.LogWarning("Failed to find a projection of type '{type}' with the specified id '{id}'", relatedProjectionType.Name, foreignKey.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
                            continue;
                        }
                        relatedProjection.RemoveFrom(relationship.Path, id);
                        await relatedProjections.UpdateAsync(relatedProjection, cancellationToken).ConfigureAwait(false);
                        break;
                }
            }
        }
    }

    /// <inheritdoc/>
    public virtual Task<long> CountAsync(CancellationToken cancellationToken = default) => CountAsync(Builders<BsonDocument>.Filter.Empty, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<long> CountAsync(FilterDefinition<BsonDocument> filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Projections.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }
        catch (MongoCommandException ex) when(ex.CodeName == "IndexNotFound")
        {
            return 0;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<BsonDocument> ToListAsync([EnumeratorCancellation] CancellationToken cancellationToken = default) 
    {
        using var cursor = await Projections.FindAsync(Builders<BsonDocument>.Filter.Empty, new FindOptions<BsonDocument, BsonDocument>(), cancellationToken).ConfigureAwait(false);
        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false)) foreach (var doc in cursor.Current) yield return doc;
    }

    /// <inheritdoc/>
    public virtual IQueryable<BsonDocument> AsQueryable() => Projections.AsQueryable();

}