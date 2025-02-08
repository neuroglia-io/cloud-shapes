var builder = WebAssemblyHostBuilder.CreateDefault(args);

var serializationOptionsSetup = JsonSerializer.DefaultOptionsConfiguration;
JsonSerializer.DefaultOptionsConfiguration = (options) =>
{
    serializationOptionsSetup(options);
    options.WriteIndented = true;
    options.Converters.Add(new ObjectConverter());
};

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.Configure(JsonSerializer.DefaultOptionsConfiguration);
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

await builder.Build().RunAsync();