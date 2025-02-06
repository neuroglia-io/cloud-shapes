using CloudShapes.Application.Queries;
using CloudShapes.Application.Queries.Projections;

namespace CloudShapes.Api.Controllers;

/// <summary>
/// Represents the controller used to manage projections
/// </summary>
/// <param name="mediator">The service used to mediate calls</param>
[Route(ApiDefaults.Routing.RoutePrefix)]
public class ProjectionsController(IMediator mediator)
    : Controller
{

    /// <summary>
    /// List projections of the specified type
    /// </summary>
    /// <param name="type">The name of the type of the projections to list</param>
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

}
