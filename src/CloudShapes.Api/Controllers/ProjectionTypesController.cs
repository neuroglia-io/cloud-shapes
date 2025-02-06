using CloudShapes.Application.Commands.ProjectionTypes;
using CloudShapes.Application.Queries;
using CloudShapes.Application.Queries.ProjectionTypes;

namespace CloudShapes.Api.Controllers;

/// <summary>
/// Represents the controller used to manage projection types
/// </summary>
/// <param name="mediator">The service used to mediate calls</param>
[Route($"{ApiDefaults.Routing.RoutePrefix}/projections/types")]
public class ProjectionTypesController(IMediator mediator)
    : Controller
{

    /// <summary>
    /// Gets the projection type with the specified name
    /// </summary>
    /// <param name="name">The name of the projection type to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(ProjectionType), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetProjectionType(string name, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new GetProjectionTypeQuery(name), cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Creates a new projection type
    /// </summary>
    /// <param name="queryOptions">The query options</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProjectionType>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ListProjectionTypes([FromQuery] QueryOptions queryOptions, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new ListProjectionTypesQuery(queryOptions), cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Creates a new projection type
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProjectionType([FromBody] CreateProjectionTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Deletes the specified projection type
    /// </summary>
    /// <param name="name">The name of the projection type to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteProjectionType(string name, CancellationToken cancellationToken = default)
    {
        var result = await mediator.ExecuteAsync(new DeleteProjectionTypeCommand(name), cancellationToken).ConfigureAwait(false);
        return this.Process(result, (int)HttpStatusCode.NoContent);
    }

}
