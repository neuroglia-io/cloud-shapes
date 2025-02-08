namespace CloudShapes.Application.Queries.Projections;

/// <summary>
/// Represents the query used to delete a specific projection
/// </summary>
/// <param name="type">The type of the projection to delete</param>
/// <param name="id">The id of the projection to delete</param>
public class DeleteProjectionQuery(string type, string id)
    : Query<object>
{

    /// <summary>
    /// Gets the type of the projection to delete
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets the id of the projection to delete
    /// </summary>
    public virtual string Id { get; } = id;

}

/// <summary>
/// Represents the service used to handle <see cref="DeleteProjectionQuery"/> instances
/// </summary>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
public class DeleteProjectionQueryHandler(IDbContext dbContext)
    : IQueryHandler<DeleteProjectionQuery, object>
{

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<object>> HandleAsync(DeleteProjectionQuery query, CancellationToken cancellationToken = default)
    {
        var set = DbContext.Set(query.Type);
        await set.DeleteAsync(query.Id, cancellationToken).ConfigureAwait(false);
        return this.Ok();
    }

}
