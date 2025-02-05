using CloudShapes.Application.Commands.ProjectionTypes;

namespace CloudShapes.Api.Controllers;

/// <summary>
/// Represents the controller used to manage projections
/// </summary>
/// <param name="mediator">The service used to mediate calls</param>
[Route("api/v1/[controller]")]
public class ProjectionsController(IMediator mediator)
    : Controller
{

    /// <summary>
    /// Creates a new projection type
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPost("types")]
    public async Task<IActionResult> CreateProjectionType([FromBody]CreateProjectionTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

}
