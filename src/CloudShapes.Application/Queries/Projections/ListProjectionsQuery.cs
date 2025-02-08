namespace CloudShapes.Application.Queries.Projections;

/// <summary>
/// Represents the query used to list <see cref="ProjectionType"/>s
/// </summary>
/// <param name="type">The name of the type of projections to list</param>
/// <param name="options">The query options</param>
public class ListProjectionsQuery(string type, QueryOptions options)
    : Query<PagedResult<object>>
{

    /// <summary>
    /// Gets the name of the type of projections to list
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets the query options
    /// </summary>
    public virtual QueryOptions Options { get; } = options;

}

/// <summary>
/// Represents the service used to handle <see cref="ListProjectionsQuery"/> instances
/// </summary>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
public class ListProjectionsQueryHandler(IDbContext dbContext, IJsonSerializer jsonSerializer)
    : IQueryHandler<ListProjectionsQuery, PagedResult<object>>
{

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult<PagedResult<object>>> HandleAsync(ListProjectionsQuery query, CancellationToken cancellationToken = default)
    {
        var set = DbContext.Set(query.Type);
        var filter = query.Options.BuildFilter<BsonDocument>(set.Type);
        var totalCount = await set.CountAsync(filter, cancellationToken).ConfigureAwait(false);
        var cursor = await set.FindAsync(filter, cancellationToken).ConfigureAwait(false);
        var results = cursor.ToAsyncEnumerable(cancellationToken);
        if (query.Options.Skip.HasValue) results = results.Skip(query.Options.Skip.Value);
        if (query.Options.Limit.HasValue) results = results.Take(query.Options.Limit.Value);
        var items = await results.Select(BsonTypeMapper.MapToDotNetValue).ToListAsync(cancellationToken).ConfigureAwait(false);
        var pageResult = new PagedResult<object>(items, totalCount, query.Options.Limit, query.Options.Limit.HasValue ? ((query.Options.Skip ?? 0) / query.Options.Limit.Value) + 1 : null);
        return this.Ok(pageResult);
    }

}
