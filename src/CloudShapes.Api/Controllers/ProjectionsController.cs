﻿// Copyright © 2025-Present The Cloud Shapes Authors
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

using CloudShapes.Integration.Commands.Projections;
using CloudShapes.Integration.Queries.Projections;

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
    /// Creates a new projection
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPost("projections")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateProjection([FromBody] CreateProjectionCommand command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Gets the specified projection
    /// </summary>
    /// <param name="type">The name of the type of the projection to get</param>
    /// <param name="id">The id of the projection to get</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpGet("{type}/{id}")]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
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
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> ListProjections(string type, [FromQuery] QueryOptions queryOptions, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new ListProjectionsQuery(type, queryOptions), cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Updates an existing projection
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPut]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateProjection([FromBody]UpdateProjectionCommand command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        return this.Process(result);
    }

    /// <summary>
    /// Patches an existing projection
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IActionResult"/> that describes the result of the operation</returns>
    [HttpPatch]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> PatchProjection([FromBody] PatchProjectionCommand command, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
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
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteProjection(string type, string id, CancellationToken cancellationToken = default)
    {
        if (!this.ModelState.IsValid) return this.ValidationProblem(this.ModelState);
        var result = await mediator.ExecuteAsync(new DeleteProjectionCommand(type, id), cancellationToken).ConfigureAwait(false);
        return this.Process(result, (int)HttpStatusCode.NoContent);
    }

}
