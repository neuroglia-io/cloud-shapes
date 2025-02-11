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
    options.Converters.Add(new ObjectConverter());
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