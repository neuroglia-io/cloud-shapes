using CloudShapes.Application.Commands.CloudEvents;

namespace CloudShapes.Api.Controllers;

/// <summary>
/// Represents the controller used to manage CloudEvents
/// </summary>
/// <param name="mediator">The service used to mediate calls</param>
[ApiController, Route($"{ApiDefaults.Routing.RoutePrefix}/[controller]")]
public class EventsController(IMediator mediator)
    : Controller
{

    /// <summary>
    /// Ingests the specified <see cref="CloudEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to ingest</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPost("pub")]
    public async Task<IActionResult> IngestCloudEvent([FromBody]CloudEvent e, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new IngestCloudEventCommand(e), cancellationToken).ConfigureAwait(false);
        return this.Process(result, (int)HttpStatusCode.Accepted);
    }

}
