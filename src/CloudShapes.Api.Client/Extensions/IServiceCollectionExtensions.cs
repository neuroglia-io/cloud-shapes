namespace CloudShapes.Api.Client;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a client for the Cloud Shapes HTTP API
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setup">An <see cref="Action{T}"/> used to configure the <see cref="CloudShapesApiClientOptions"/> to use</param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddCloudShapesApiClient(this IServiceCollection services, Action<CloudShapesApiClientOptions> setup)
    {
        services.Configure(setup);
        services.AddHttpClient<ICloudShapesApiClient, CloudShapesApiClient>((provider, http) =>
        {
            var apiClientOptions = provider.GetRequiredService<IOptions<CloudShapesApiClientOptions>>().Value;
            http.BaseAddress = apiClientOptions.BaseAddress;
        });
        services.TryAddSingleton<IPluralize>(provider => new Pluralizer());
        return services;
    }

}
