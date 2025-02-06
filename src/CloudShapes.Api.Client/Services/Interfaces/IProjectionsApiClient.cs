namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes Projections API
/// </summary>
public interface IProjectionsApiClient
{

    /// <summary>
    /// Gets the specified projection
    /// </summary>
    /// <param name="type">The type of the projection to get</param>
    /// <param name="id">The id of the projection to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The specified projection</returns>
    Task<object> GetAsync(string type, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists projections of the specified type
    /// </summary>
    /// <param name="type">The type of projections to list</param>
    /// <param name="queryOptions">The query's options</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="PagedResult{T}"/> that wraps filtered projections</returns>
    Task<PagedResult<object>> ListAsync(string type, QueryOptions? queryOptions = null, CancellationToken cancellationToken = default);

}