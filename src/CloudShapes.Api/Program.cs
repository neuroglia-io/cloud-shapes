var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddResponseCompression();
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        JsonSerializer.DefaultOptionsConfiguration(options.JsonSerializerOptions);
        options.JsonSerializerOptions.Converters.Add(new ObjectConverter());
    });
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddMediator(options =>
{
    options.ScanAssembly(typeof(CloudShapes.Application.Commands.ProjectionTypes.CreateProjectionTypeCommandHandler).Assembly);
});
builder.Services.Configure<ApplicationOptions>(builder.Configuration);
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    BsonSerializer.RegisterSerializer(new JsonSchemaBsonSerializer());
    BsonSerializer.RegisterSerializer(new DateTimeOffsetBsonSerializer());
    ConventionRegistry.Register("ApplicationConventions", new ConventionPack { new CamelCaseElementNameConvention(), new IgnoreIfNullConvention(true) }, type => true);
    return new MongoClient(options.Database.ConnectionString);
});
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(provider.GetRequiredService<IOptions<ApplicationOptions>>().Value.Database.Name));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<ProjectionType>($"{nameof(ProjectionType)}s"));
builder.Services.AddSingleton<IPluralize>(provider => new Pluralizer());
builder.Services.AddSerialization();
builder.Services.AddJQExpressionEvaluator();
builder.Services.AddCloudEventBus();
builder.Services.AddSingleton<ICloudEventCorrelationKeyResolver, CloudEventCorrelationKeyResolver>();
builder.Services.AddHostedService<CloudEventHubDispatcher>();
builder.Services.AddSingleton<IPatchHandler, JsonMergePatchHandler>();
builder.Services.AddSingleton<IPatchHandler, JsonPatchHandler>();
builder.Services.AddSingleton<IPatchHandler, JsonStrategicMergePatchHandler>();
builder.Services.AddSingleton<ISchemaValidator, SchemaValidator>();
builder.Services.AddSingleton<IDbContext, DbContext>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
app.UseResponseCompression();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapOpenApi();
app.MapScalarApiReference("/api/doc", options =>
{
    options.WithTitle("Cloud Shapes API");
});
app.MapControllers();
app.MapHub<CloudEventHub>("api/ws/events");
app.MapFallbackToFile("index.html");


await app.RunAsync();