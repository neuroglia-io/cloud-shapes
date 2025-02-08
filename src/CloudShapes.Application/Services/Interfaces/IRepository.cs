namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage projections of a specific type
/// </summary>
public interface IRepository
{

    /// <summary>
    /// Gets the type of projections managed by the <see cref="IRepository"/>
    /// </summary>
    ProjectionType Type { get; }

    /// <summary>
    /// Determines whether or not the specified projection exists
    /// </summary>
    /// <param name="id">The id of the projection to check</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A boolean indicating whether or not the specified projection exists</returns>
    Task<bool> ContainsAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the projection with the specified id
    /// </summary>
    /// <param name="id">The id of the projection to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The projection with the specified id</returns>
    Task<BsonDocument?> GetAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds projections that match the specified filter
    /// </summary>
    /// <param name="filter">The filter to match</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncCursor{TDocument}"/> used to enumerate matches</returns>
    Task<IAsyncCursor<BsonDocument>> FindAsync(FilterDefinition<BsonDocument> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the specified projection
    /// </summary>
    /// <param name="projection">The projection to add</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task AddAsync(BsonDocument projection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified projection
    /// </summary>
    /// <param name="projection">The updated projection</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task UpdateAsync(BsonDocument projection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified projection
    /// </summary>
    /// <param name="id">The id of the projection to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of projections in the collection
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The count of projections in the collection</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of projections in the collection that match the specified filter
    /// </summary>
    /// <param name="filter">The filter definition used to match documents</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The count of projections in the collection that match the specified filter</returns>
    Task<long> CountAsync(FilterDefinition<BsonDocument> filter, CancellationToken cancellationToken= default);

    /// <summary>
    /// Lists all projections contained by the repository
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/> used to asynchronously enumerate all projections contained by the repository</returns>
    IAsyncEnumerable<BsonDocument> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a queryable source of projections
    /// </summary>
    /// <returns>A new <see cref="IQueryable{T}"/></returns>
    IQueryable<BsonDocument> AsQueryable();

}
