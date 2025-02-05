var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddMediator(options =>
{
    options.ScanAssembly(typeof(CloudShapes.Application.Commands.ProjectionTypes.CreateProjectionTypeCommand).Assembly);
});
builder.Services.Configure<ApplicationOptions>(builder.Configuration);
builder.Services.AddSingleton<IMongoClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    return new MongoClient(options.Database.ConnectionString);
});
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(provider.GetRequiredService<IOptions<ApplicationOptions>>().Value.Database.Name));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<ProjectionType>($"{nameof(ProjectionType)}s"));
builder.Services.AddSingleton<IPluralize>(provider => new Pluralizer());
builder.Services.AddSerialization();
builder.Services.AddNewtonsoftJsonSerializer();
builder.Services.AddJQExpressionEvaluator();
builder.Services.AddSingleton<ICloudEventValueResolver, CloudEventValueResolver>();

var app = builder.Build();
BsonSerializer.RegisterSerializer(new ObjectSerializer(x => true));

app.MapControllers();

await app.RunAsync();