namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage projection repositories
/// </summary>
public interface IDbContext
{

    /// <summary>
    /// Gets the <see cref="IRepository"/> used to manage projections of the specified type
    /// </summary>
    /// <param name="typeName">The name of the type of projection to get the repository for</param>
    /// <returns>The <see cref="IRepository"/> used to manage projections of the specified type</returns>
    IRepository Set(string typeName);

    /// <summary>
    /// Gets the <see cref="IRepository"/> used to manage projections of the specified type
    /// </summary>
    /// <param name="type">The type of projection to get the repository for</param>
    /// <returns>The <see cref="IRepository"/> used to manage projections of the specified type</returns>
    IRepository Set(ProjectionType type);

}
