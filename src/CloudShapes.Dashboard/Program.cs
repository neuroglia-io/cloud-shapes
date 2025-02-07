var builder = WebAssemblyHostBuilder.CreateDefault(args);
JsonConvert.DefaultSettings = () =>
{
    var settings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        DateParseHandling = DateParseHandling.DateTimeOffset
    };
    settings.Converters.Add(new ObjectConverter());
    return settings;
};

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddLogging();
builder.Services.AddSerialization();
builder.Services.AddNewtonsoftJsonSerializer();
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
builder.Services.AddSingleton<IGraphLayoutService, GraphLayoutService>();
builder.Services.AddSingleton<IBreadcrumbManager, BreadcrumbManager>();
builder.Services.AddCloudShapesApiClient(options =>
{
    options.BaseAddress = new(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();