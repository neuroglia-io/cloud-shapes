var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddLogging();
builder.Services.AddSerialization();
builder.Services.AddNewtonsoftJsonSerializer();
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

await builder.Build().RunAsync();