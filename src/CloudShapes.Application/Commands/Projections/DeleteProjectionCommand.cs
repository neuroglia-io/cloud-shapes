namespace CloudShapes.Application.Commands.Projections;

/// <summary>
/// Represents the service used to handle <see cref="DeleteProjectionCommand"/>s
/// </summary>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
public class DeleteProjectionCommandHandler(IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext)
    : ICommandHandler<DeleteProjectionCommand>
{

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to manage <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult> HandleAsync(DeleteProjectionCommand command, CancellationToken cancellationToken = default)
    {
        var type = await (await ProjectionTypes.FindAsync(Builders<ProjectionType>.Filter.Eq("_id", command.Type), new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
           ?? throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, $"Failed to find a projection type with the specified name '{command.Type}'"));
        var set = DbContext.Set(type);
        if (!await set.ContainsAsync(command.Id, cancellationToken).ConfigureAwait(false)) throw new ProblemDetailsException(new(Problems.Types.NotFound, Problems.Titles.NotFound, Problems.Statuses.NotFound, $"Failed to find a projection of type '{command.Type}' with the specified id '{command.Id}'"));
        await set.DeleteAsync(command.Id, cancellationToken).ConfigureAwait(false);
        return this.Ok();
    }

}
