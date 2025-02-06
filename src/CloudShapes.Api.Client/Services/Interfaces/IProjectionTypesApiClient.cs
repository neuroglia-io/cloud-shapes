namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes Projection Types API
/// </summary>
public interface IProjectionTypesApiClient
{

    /// <summary>
    /// Creates a new <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The newly created <see cref="ProjectionType"/></returns>
    Task<ProjectionType> CreateAsync(CreateProjectionTypeCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the <see cref="ProjectionType"/> with the specified name
    /// </summary>
    /// <param name="name">The name of the <see cref="ProjectionType"/> to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="ProjectionType"/> with the specified name</returns>
    Task<ProjectionType> GetAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists <see cref="ProjectionType"/>s
    /// </summary>
    /// <param name="queryOptions">The query's options</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="PagedResult{T}"/> that wraps filtered <see cref="ProjectionType"/>s</returns>
    Task<PagedResult<ProjectionType>> ListAsync(QueryOptions? queryOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the <see cref="ProjectionType"/> with the specified name
    /// </summary>
    /// <param name="name">The name of the <see cref="ProjectionType"/> to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);

}
