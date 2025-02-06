namespace CloudShapes.Application.Queries.ProjectionTypes;

/// <summary>
/// Represents the query used to list <see cref="ProjectionType"/>s
/// </summary>
/// <param name="options">The query options</param>
public class ListProjectionTypesQuery(QueryOptions options)
    : Query<PagedResult<ProjectionType>>
{

    /// <summary>
    /// Gets the query options
    /// </summary>
    public virtual QueryOptions Options { get; } = options;

}

/// <summary>
/// Represents the service used to handle <see cref="ListProjectionTypesQuery"/> instances
/// </summary>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
public class ListProjectionTypesQueryHandler(IMongoCollection<ProjectionType> projectionTypes)
    : IQueryHandler<ListProjectionTypesQuery, PagedResult<ProjectionType>>
{

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<PagedResult<ProjectionType>>> HandleAsync(ListProjectionTypesQuery query, CancellationToken cancellationToken = default)
    {
        var filter = query.Options.BuildFilter<ProjectionType>();
        var totalCount = await ProjectionTypes.CountDocumentsAsync(filter, new CountOptions(), cancellationToken).ConfigureAwait(false);
        var cursor = await ProjectionTypes.FindAsync(filter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false);
        var results = cursor.ToAsyncEnumerable(cancellationToken);
        if (query.Options.Skip.HasValue) results = results.Skip(query.Options.Skip.Value);
        if (query.Options.Limit.HasValue) results = results.Take(query.Options.Limit.Value);
        var items = await results.ToListAsync(cancellationToken).ConfigureAwait(false);
        var pageResult = new PagedResult<ProjectionType>(items, totalCount, query.Options.Limit, query.Options.Limit.HasValue ? ((query.Options.Skip ?? 0) / query.Options.Limit.Value) + 1 : null);
        return this.Ok(pageResult);
    }

}
