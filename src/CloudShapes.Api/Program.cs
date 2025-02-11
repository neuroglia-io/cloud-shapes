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
    ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, type => true);
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