// Copyright © 2025-Present The Cloud Shapes Authors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CloudShapes.Integration.Commands.ProjectionTypes;
using CloudShapes.Integration.Queries.ProjectionTypes;

namespace CloudShapes.Api.Controllers;

/// <summary>
/// Represents the controller used to manage projection types
/// </summary>
/// <param name="mediator">The service used to mediate calls</param>
[ApiController, Route($"{ApiDefaults.Routing.RoutePrefix}/projections/types")]
public class ProjectionTypesController(IMediator mediator)
    : Controller
{

    /// <summary>
    /// Creates a new projection type
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectionType), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateProjectionType([FromBody] CreateProjectionTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Gets the projection type with the specified name
    /// </summary>
    /// <param name="name">The name of the projection type to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(ProjectionType), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
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
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> ListProjectionTypes([FromQuery] QueryOptions queryOptions, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new ListProjectionTypesQuery(queryOptions), cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Migrates the schema of the specified projection type
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns></returns>
    [HttpPut("migrate")]
    [ProducesResponseType(typeof(ProjectionTypeSchemaMigrationResult), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> MigrateProjectionType([FromBody] MigrateProjectionTypeSchemaCommand command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteProjectionType(string name, CancellationToken cancellationToken = default)
    {
        var result = await mediator.ExecuteAsync(new DeleteProjectionTypeCommand(name), cancellationToken).ConfigureAwait(false);
        return this.Process(result, (int)HttpStatusCode.NoContent);
    }

}
