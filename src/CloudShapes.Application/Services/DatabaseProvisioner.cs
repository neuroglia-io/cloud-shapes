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

using CloudShapes.Integration.Commands.ProjectionTypes;
using System.Diagnostics;

namespace CloudShapes.Application.Services;

/// <summary>
/// Represents a service used to provision the application's database using seed files
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="options">The service used to access the current <see cref="ApplicationOptions"/></param>
/// <param name="mediator">The service used to mediate calls</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="yamlSerializer">The service used to serialize/deserialize data to/from YAML</param>
/// <param name="mongo">The service used to interact with Mongo</param>
/// <param name="projectionTypes">The service used to manage <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The application's <see cref="IDbContext"/></param>
/// <param name="pluralize">The service used to pluralize words</param>
public class DatabaseProvisioner(ILogger<DatabaseProvisioner> logger, IOptions<ApplicationOptions> options, IMediator mediator, IJsonSerializer jsonSerializer, IYamlSerializer yamlSerializer, IMongoClient mongo, IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext, IPluralize pluralize)
    : IHostedService
{

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Gets the current <see cref="ApplicationOptions"/>
    /// </summary>
    protected ApplicationOptions Options { get; } = options.Value;

    /// <summary>
    /// Gets the service used to mediate calls
    /// </summary>
    protected IMediator Mediator { get; } = mediator;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from YAML
    /// </summary>
    protected IYamlSerializer YamlSerializer { get; } = yamlSerializer;

    /// <summary>
    /// Gets the service used to interact with Mongo
    /// </summary>
    protected IMongoClient Mongo { get; } = mongo;

    /// <summary>
    /// Gets the service used to manage <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the application's <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to pluralize words
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <inheritdoc/>
    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        var databases = await (await Mongo.ListDatabaseNamesAsync(cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        if (databases.Any(db => db.Equals(Options.Database.Name, StringComparison.OrdinalIgnoreCase)))
        {
            Logger.LogInformation("The database with name '{name}' already exists. Skipping provisioning", Options.Database.Name);
            return;
        }
        var directory = new DirectoryInfo(Options.Database.Provisioning.Directory);
        if (!directory.Exists)
        {
            Logger.LogWarning("The directory '{directory}' does not exist or cannot be found. Skipping provisioning the database", directory.FullName);
            return;
        }
        await ProvisionProjectionTypesAsync(cancellationToken).ConfigureAwait(false);
        await ProvisionProjectionsAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Provisions <see cref="ProjectionType"/>s
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProvisionProjectionTypesAsync(CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        var directory = new DirectoryInfo(Path.Combine(Options.Database.Provisioning.Directory, "types"));
        if (!directory.Exists) return;
        Logger.LogInformation("Starting importing projection types from directory '{directory}'...", directory.FullName);
        var files = directory.GetFiles(Options.Database.Provisioning.FilePattern, SearchOption.AllDirectories).Where(f => f.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || f.FullName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase) || f.FullName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase));
        if (!files.Any())
        {
            Logger.LogWarning("No projection type static files matching search pattern '{pattern}' found in directory '{directory}'. Skipping import.", Options.Database.Provisioning.FilePattern, directory.FullName);
            return;
        }
        stopwatch.Restart();
        var count = 0;
        foreach (var file in files)
        {
            try
            {
                var extension = file.FullName.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                var serializer = extension?.ToLowerInvariant() switch
                {
                    "json" => (ITextSerializer)JsonSerializer,
                    "yml" or "yaml" => YamlSerializer,
                    _ => throw new NotSupportedException($"The specified extension '{extension}' is not supported for static files")
                };
                using var stream = file.OpenRead();
                using var streamReader = new StreamReader(stream);
                var text = await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                var type = serializer.Deserialize<ProjectionType>(text)!;
                var command = new CreateProjectionTypeCommand()
                {
                    Name = type.Name,
                    Summary = type.Summary,
                    Description = type.Description,
                    Schema = type.Schema,
                    Tags = type.Tags,
                    Triggers = type.Triggers,
                    Relationships = type.Relationships,
                    Indexes = type.Indexes
                };
                await Mediator.ExecuteAndUnwrapAsync(command, cancellationToken).ConfigureAwait(false);
                Logger.LogInformation("Successfully imported projection type with name '{type}' from file '{file}'", $"{type.Name}", file.FullName);
                count++;
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occurred while reading a projection type from file '{file}': {ex}", file.FullName, ex);
                continue;
            }
        }
        stopwatch.Stop();
        Logger.LogInformation("Completed importing {count} projection types in {ms} milliseconds", count, stopwatch.Elapsed.TotalMilliseconds);
    }

    /// <summary>
    /// Provisions <see cref="ProjectionType"/>s
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProvisionProjectionsAsync(CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        var directory = new DirectoryInfo(Path.Combine(Options.Database.Provisioning.Directory, "projections"));
        if (!directory.Exists) return;
        var totalCount = 0;
        stopwatch.Start();
        Logger.LogInformation("Starting importing projections from directory '{directory}'...", directory.FullName);
        var projectionTypes = await (await ProjectionTypes.FindAsync(Builders<ProjectionType>.Filter.Empty, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        var lookup = projectionTypes.ToDictionary(pt => pt.Name, pt => pt);
        var projectionTypeDirectories = new Dictionary<ProjectionType, DirectoryInfo>();
        var graph = new Dictionary<string, List<string>>();
        var indegree = new Dictionary<string, int>();
        foreach (var subdirectory in directory.GetDirectories())
        {
            var typeName = subdirectory.Name;
            if (Pluralize.IsPlural(typeName)) typeName = Pluralize.Singularize(typeName);
            var type = projectionTypes.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            if (type == null)
            {
                Logger.LogWarning("Failed to find a projection type with name '{type}'. Skipping importing projections from directory '{directory}'", directory.Name, directory.FullName);
                continue;
            }
            projectionTypeDirectories[type] = subdirectory;
        }
        foreach (var projectionType in projectionTypes)
        {
            graph[projectionType.Name] = [];
            indegree[projectionType.Name] = 0;
        }
        foreach (var projectionType in projectionTypes)
        {
            if (projectionType.Relationships == null) continue;
            foreach (var relationship in projectionType.Relationships)
            {
                if (!graph.ContainsKey(relationship.Target))
                {
                    Logger.LogWarning($"Target '{relationship.Target}' referenced in '{projectionType.Name}' not found.");
                    continue;
                }
                if (relationship.Type.Equals(ProjectionRelationshipType.OneToOne, StringComparison.OrdinalIgnoreCase))
                {
                    graph[relationship.Target].Add(projectionType.Name);
                    indegree[projectionType.Name]++;
                }
                else if (relationship.Type.Equals(ProjectionRelationshipType.OneToMany, StringComparison.OrdinalIgnoreCase))
                {
                    graph[projectionType.Name].Add(relationship.Target);
                    indegree[relationship.Target]++;
                }
                else
                {
                    Logger.LogWarning($"Unknown/unsupported relationship type '{relationship.Type}' in '{projectionType.Name}'");
                }
            }
        }
        var queue = new Queue<string>();
        foreach (var kvp in indegree) if (kvp.Value == 0)  queue.Enqueue(kvp.Key);
        var processingOrder = new List<string>();
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            processingOrder.Add(current);
            var projectionType = lookup[current];
            if (projectionTypeDirectories.TryGetValue(projectionType, out directory) && directory != null) totalCount += await ProvisionProjectionsAsync(projectionType, directory, cancellationToken).ConfigureAwait(false);
            foreach (var neighbor in graph[current])
            {
                indegree[neighbor]--;
                if (indegree[neighbor] == 0) queue.Enqueue(neighbor);
            }
        }
        if (processingOrder.Count != projectionTypes.Count) Logger.LogError("Cycle detected or some projection types could not be processed.");
        stopwatch.Stop();
        Logger.LogInformation("Completed importing {count} projections in {ms} milliseconds", totalCount, stopwatch.Elapsed.TotalMilliseconds);
    }

    /// <summary>
    /// Provisions <see cref="ProjectionType"/>s
    /// </summary>
    /// <param name="type">The type of projections to provision</param>
    /// <param name="directory">The directory that contains the files of the projections to provision</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The amount of imported projections</returns>
    protected virtual async Task<int> ProvisionProjectionsAsync(ProjectionType type, DirectoryInfo directory, CancellationToken cancellationToken = default)
    {
        var typeStopwatch = new Stopwatch();
        var files = directory.GetFiles(Options.Database.Provisioning.FilePattern, SearchOption.AllDirectories).Where(f => f.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || f.FullName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase) || f.FullName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase));
        if (!files.Any())
        {
            Logger.LogWarning("No projection static files matching search pattern '{pattern}' found in directory '{directory}'. Skipping import.", Options.Database.Provisioning.FilePattern, directory.FullName);
            return 0;
        }
        var set = DbContext.Set(type);
        Logger.LogInformation("Starting importing projections of type '{type}' from directory '{directory}'...", type.Name, directory.FullName);
        typeStopwatch.Restart();
        var count = 0;
        foreach (var file in files)
        {
            try
            {
                var extension = file.FullName.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                using var stream = file.OpenRead();
                using var streamReader = new StreamReader(stream);
                var text = await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                var projection = extension?.ToLowerInvariant() switch
                {
                    "json" => BsonDocument.Parse(text),
                    "yml" or "yaml" => BsonDocument.Parse(YamlSerializer.ConvertToJson(text)),
                    _ => throw new NotSupportedException($"The specified extension '{extension}' is not supported for static files")
                };
                await set.AddAsync(projection, cancellationToken).ConfigureAwait(false);
                Logger.LogInformation("Successfully imported projection of '{type}' with id '{id}' from file '{file}'", type.Name, projection["_id"], file.FullName);
                count++;
            }
            catch (Exception ex)
            {
                Logger.LogError("An error occurred while reading a projection of '{type}' from file '{file}': {ex}", type.Name, file.FullName, ex);
                return 0;
            }
        }
        typeStopwatch.Stop();
        Logger.LogInformation("Completed importing {count} projections from directory '{directory}' in {ms} milliseconds", count, directory.FullName, typeStopwatch.Elapsed.TotalMilliseconds);
        return count;
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
