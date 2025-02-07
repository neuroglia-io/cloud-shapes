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
        var path = $"{PathPrefix}/{Pluralize.Pluralize(type).ToCamelCase()}";
        if (queryOptions != null) path += "?" + queryOptions.ToQueryString();
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var response = await ProcessResponseAsync(await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<PagedResult<IDictionary<string, object>>>(json)!;
    }

}