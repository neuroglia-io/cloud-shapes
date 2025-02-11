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

namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IEventsApiClient"/> interface
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="httpClient">The service used to perform HTTP requests</param>
public class EventsApiClient(ILogger<EventsApiClient> logger, IJsonSerializer jsonSerializer, HttpClient httpClient)
    : ApiClientBase(logger, jsonSerializer, httpClient), IEventsApiClient
{

    const string PathPrefix = "api/events";

    /// <inheritdoc/>
    public virtual async Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e);
        var json = JsonSerializer.SerializeToText(e);
        using var content = new StringContent(json, Encoding.UTF8, CloudEventContentType.Json);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{PathPrefix}/pub") { Content = content };
        using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await ProcessResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

}

/// <summary>
/// Represents the base class for all API clients
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="httpClient">The service used to perform HTTP requests</param>
public abstract class ApiClientBase(ILogger logger, IJsonSerializer jsonSerializer, HttpClient httpClient)
{

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <summary>
    /// Gets the service used to perform HTTP requests
    /// </summary>
    protected HttpClient HttpClient { get; } = httpClient;

    /// <summary>
    /// Processes the specified <see cref="HttpResponseMessage"/>
    /// </summary>
    /// <param name="response">the <see cref="HttpResponseMessage"/> to process</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The processed <see cref="HttpResponseMessage"/></returns>
    protected virtual async Task<HttpResponseMessage> ProcessResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (response.IsSuccessStatusCode) return response;
        var content = string.Empty;
        if (response.Content != null) content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        this.Logger.LogError("The remote server responded with a non-success status code '{statusCode}': {errorDetails}", response.StatusCode, content);
        if (!response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(content)) response.EnsureSuccessStatusCode();
            else throw new ProblemDetailsException(this.JsonSerializer.Deserialize<ProblemDetails>(content)!);
        }
        return response;
    }

}