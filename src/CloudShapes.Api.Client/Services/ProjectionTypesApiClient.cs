namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IProjectionTypesApiClient"/> interface
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="httpClient">The service used to perform HTTP requests</param>
public class ProjectionTypesApiClient(ILogger<ProjectionTypesApiClient> logger, IJsonSerializer jsonSerializer, HttpClient httpClient)
    : ApiClientBase(logger, jsonSerializer, httpClient), IProjectionTypesApiClient
{

    const string PathPrefix = "api/projections/types";

    /// <inheritdoc/>
    public virtual async Task<ProjectionType> CreateAsync(CreateProjectionTypeCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var json = JsonSerializer.SerializeToText(command);
        using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        using var request = new HttpRequestMessage(HttpMethod.Post, PathPrefix) { Content = content };
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProjectionType>(json)!;
    }

    /// <inheritdoc/>
    public virtual async Task<ProjectionType> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{PathPrefix}/{name}");
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<ProjectionType>(json)!;
    }

    /// <inheritdoc/>
    public virtual async Task<PagedResult<ProjectionType>> ListAsync(QueryOptions? queryOptions = null, CancellationToken cancellationToken = default)
    {
        var path = PathPrefix;
        if (queryOptions != null) path += "?" + queryOptions.ToQueryString();
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<PagedResult<ProjectionType>>(json)!;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"{PathPrefix}/{name}");
        await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
    }

}
