namespace CloudShapes.Application.Queries.ProjectionTypes;

/// <summary>
/// Represents the query used to get a specific projection type
/// </summary>
/// <param name="name">The name of the projection type to get</param>
public class GetProjectionTypeQuery(string name)
    : Query<ProjectionType>
{

    /// <summary>
    /// Gets the name of the projection to get
    /// </summary>
    public virtual string Name { get; } = name;

}

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