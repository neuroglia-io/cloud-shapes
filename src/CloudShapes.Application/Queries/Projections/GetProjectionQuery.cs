namespace CloudShapes.Application.Queries.Projections;

/// <summary>
/// Represents the query used to get a specific projection
/// </summary>
/// <param name="type">The type of the projection to get</param>
/// <param name="id">The id of the projection to get</param>
public class GetProjectionQuery(string type, string id)
    : Query<object>
{

    /// <summary>
    /// Gets the type of the projection to get
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets the id of the projection to get
    /// </summary>
    public virtual string Id { get; } = id;

}

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