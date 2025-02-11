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
        services.TryAddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<CloudShapesApiClientOptions>>().Value;
            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri(options.BaseAddress, "api/ws/events"))
                .WithAutomaticReconnect()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };
                    options.PayloadSerializerOptions.Converters.Add(new ObjectConverter());
                })
                .Build();
            return new CloudEventHubClient(connection);
        });
        services.TryAddSingleton<IPluralize>(provider => new Pluralizer());
        return services;
    }

}
