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

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    var defaultConfiguration = Neuroglia.Serialization.Json.JsonSerializer.DefaultOptionsConfiguration;
    Neuroglia.Serialization.Json.JsonSerializer.DefaultOptionsConfiguration = (serializerOptions) =>
    {
        defaultConfiguration(serializerOptions);
        serializerOptions.PropertyNameCaseInsensitive = true;
        serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull;
        serializerOptions.WriteIndented = true;
    };
    Neuroglia.Serialization.Json.JsonSerializer.DefaultOptionsConfiguration(options);
});
builder.Services.AddLogging();
builder.Services.AddSerialization();
builder.Services.AddJsonSerializer();
builder.Services.AddYamlDotNetSerializer();
builder.Services.AddScoped(provider => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddFlux(flux =>
{
    flux.ScanMarkupTypeAssembly<App>();
});
builder.Services.AddBlazorBootstrap();
builder.Services.AddSingleton<IMonacoEditorHelper, MonacoEditorHelper>();
builder.Services.AddScoped<IApplicationLayout, ApplicationLayout>();
builder.Services.AddSingleton<JSInterop>();
builder.Services.AddSingleton<MonacoInterop>();
builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
builder.Services.AddSingleton<IBreadcrumbManager, BreadcrumbManager>();
builder.Services.AddCloudShapesApiClient(options =>
{
    options.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

var app = builder.Build();
await app.RunAsync();