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

using CloudShapes.Integration.Commands.Projections;
using CloudShapes.Integration.Models;

namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IProjectionsApiClient"/> interface
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="pluralize">The service used to pluralize words</param>
/// <param name="httpClient">The service used to perform HTTP requests</param>
public class ProjectionsApiClient(ILogger<ProjectionsApiClient> logger, IJsonSerializer jsonSerializer, IPluralize pluralize, HttpClient httpClient)
    : ApiClientBase(logger, jsonSerializer, httpClient), IProjectionsApiClient
{

    const string PathPrefix = "api";

    /// <summary>
    /// Gets the service used to pluralize words
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <inheritdoc/>
    public virtual async Task<object> CreateAsync(CreateProjectionCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var json = JsonSerializer.SerializeToText(command);
        using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var request = new HttpRequestMessage(HttpMethod.Post, PathPrefix) { Content = content };
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<object>(json)!;
    }

    /// <inheritdoc/>
    public virtual async Task<object> GetAsync(string type, string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/{Pluralize.Pluralize(type).ToCamelCase()}/{id}");
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<object>(json)!;
    }

    /// <inheritdoc/>
    public virtual async Task<PagedResult<IDictionary<string, object>>> ListAsync(string type, QueryOptions? queryOptions = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        var path = $"{PathPrefix}/{Pluralize.Pluralize(type).ToCamelCase()}";
        if (queryOptions != null) path += "?" + queryOptions.ToQueryString();
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<PagedResult<IDictionary<string, object>>>(json)!;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(string type, string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{PathPrefix}/{Pluralize.Pluralize(type).ToCamelCase()}/{id}");
        await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
    }

}