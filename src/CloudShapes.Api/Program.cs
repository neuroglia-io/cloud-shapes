var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    static JsonSerializerSettings setup(JsonSerializerSettings settings)
    {
        settings.Converters.Add(new ObjectConverter());
        return settings;
    }
    JsonConvert.DefaultSettings = () =>
    {
        var settings = new JsonSerializerSettings();
        setup(settings);
        return settings;
    };
    setup(options.SerializerSettings);
});
builder.Services.AddMediator(options =>
{
    options.ScanAssembly(typeof(CloudShapes.Application.Commands.ProjectionTypes.CreateProjectionTypeCommand).Assembly);
});
builder.Services.Configure<ApplicationOptions>(builder.Configuration);
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    BsonSerializer.RegisterSerializer(new ObjectSerializer(x => true));
    ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, type => true);
    return new MongoClient(options.Database.ConnectionString);
});
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(provider.GetRequiredService<IOptions<ApplicationOptions>>().Value.Database.Name));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<ProjectionType>($"{nameof(ProjectionType)}s"));
builder.Services.AddSingleton<IPluralize>(provider => new Pluralizer());
builder.Services.AddSerialization();
builder.Services.AddNewtonsoftJsonSerializer();
builder.Services.AddJQExpressionEvaluator();
builder.Services.AddSingleton<ICloudEventCorrelationKeyResolver, CloudEventCorrelationKeyResolver>();
builder.Services.AddSingleton<IPatchHandler, JsonMergePatchHandler>();
builder.Services.AddSingleton<IPatchHandler, JsonPatchHandler>();
builder.Services.AddSingleton<IPatchHandler, JsonStrategicMergePatchHandler>();
builder.Services.AddSingleton<ISchemaValidator, SchemaValidator>();
builder.Services.AddSingleton<IDbContext, DbContext>();

var app = builder.Build();
app.MapControllers();

await app.RunAsync();