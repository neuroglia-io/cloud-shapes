using CloudShapes.Application.Queries.Projections;

namespace CloudShapes.Api.Controllers;

/// <summary>
/// Represents the controller used to manage projections
/// </summary>
/// <param name="mediator">The service used to mediate calls</param>
[ApiController, Route(ApiDefaults.Routing.RoutePrefix)]
public class ProjectionsController(IMediator mediator)
    : Controller
{

    /// <summary>
    /// Gets the specified projection
    /// </summary>
    /// <param name="type">The name of the type of the projection to get</param>
    /// <param name="id">The id of the projection to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpGet("{type}/{id}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetProjection(string type, string id, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new GetProjectionQuery(type, id), cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// List projections of the specified type
    /// </summary>
    /// <param name="type">The name of the type of the projections to list</param>
    /// <param name="queryOptions">The query options</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpGet("{type}")]
    [ProducesResponseType(typeof(PagedResult<object>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ListProjections(string type, [FromQuery] QueryOptions queryOptions, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new ListProjectionsQuery(type, queryOptions), cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Deletes the specified projection
    /// </summary>
    /// <param name="type">The name of the type of the projection to delete</param>
    /// <param name="id">The id of the projection to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpDelete("{type}/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> DeleteProjection(string type, string id, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new DeleteProjectionQuery(type, id), cancellationToken).ConfigureAwait(false);
        return this.Process(result, (int)HttpStatusCode.NoContent);
    }

}
