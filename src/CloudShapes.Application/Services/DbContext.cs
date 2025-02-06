namespace CloudShapes.Application.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IDbContext"/> interface
/// </summary>
/// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// ^<param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s</param>
/// <param name="pluralize">The service used to pluralize terms</param>
public class DbContext(IServiceProvider serviceProvider, IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IPluralize pluralize)
    : IDbContext
{

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <summary>
    /// Gets the current <see cref="IMongoDatabase"/>
    /// </summary>
    protected IMongoDatabase Database { get; } = database;

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the service used to pluralize terms
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <summary>
    /// Gets a name/value mapping of all realized <see cref="IRepository"/> instances
    /// </summary>
    protected Dictionary<string, IRepository> Repositories { get; } = [];

    /// <inheritdoc/>
    public virtual IRepository Set(string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        if (Repositories.TryGetValue(typeName, out var repository)) return repository;
        var filter = Builders<ProjectionType>.Filter.Eq("_id", BsonValue.Create(typeName));
        var type = ProjectionTypes.Find(filter, new FindOptions()).FirstOrDefault() ?? throw new NullReferenceException($"Failed to find a projection type with the specified name '{typeName}'");
        return Set(type);
    }

    /// <inheritdoc/>
    public virtual IRepository Set(ProjectionType type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (Repositories.TryGetValue(type.Name, out var repository)) return repository;
        var collectionName = Pluralize.Pluralize(type.Name);
        var collection = Database.GetCollection<BsonDocument>(collectionName);
        repository = ActivatorUtilities.CreateInstance<Repository>(ServiceProvider, type, collection);
        Repositories.Add(type.Name, repository);
        return repository;
    }

}
