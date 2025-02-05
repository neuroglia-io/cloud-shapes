using CloudShapes.Application.Services;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;

namespace CloudShapes.Application.Commands.CloudEvents;

/// <summary>
/// Represents the <see cref="Command"/> used to ingest a <see cref="CloudEvent"/>
/// </summary>
/// <param name="e">The <see cref="CloudEvent"/> to ingest</param>
public class IngestCloudEventCommand(CloudEvent e)
    : Command
{

    /// <summary>
    /// Gets the <see cref="CloudEvent"/> to ingest
    /// </summary>
    public virtual CloudEvent Event { get; } = e;

}

/// <summary>
/// Represents the service used to handle <see cref="IngestCloudEventCommand"/>s
/// </summary>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
/// <param name="pluralize">The service used to pluralize terms</param>
/// <param name="expressionEvaluator">The service used to evaluate runtime expressions</param>
/// <param name="cloudEventValueResolver">The service used to resolve values from <see cref="CloudEvent"/>s</param>
public class IngestCloudEventCommandHandler(IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IPluralize pluralize, IExpressionEvaluator expressionEvaluator, ICloudEventValueResolver cloudEventValueResolver)
    : ICommandHandler<IngestCloudEventCommand>
{

    /// <summary>
    /// Gets the current <see cref="IMongoDatabase"/>
    /// </summary>
    protected IMongoDatabase Database { get; } = database;

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the service used to pluralize terms
    /// </summary>
    protected IPluralize Pluralize { get; } = pluralize;

    /// <summary>
    /// Gets the service used to evaluate runtime expressions
    /// </summary>
    protected IExpressionEvaluator ExpressionEvaluator { get; } = expressionEvaluator;

    /// <summary>
    /// Gets the service used to resolve values from <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventValueResolver CloudEventValueResolver { get; } = cloudEventValueResolver;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult> HandleAsync(IngestCloudEventCommand command, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<ProjectionType>.Filter;
        var triggerPath = $"{nameof(ProjectionType.Triggers).ToCamelCase()}.{nameof(ProjectionTriggerCollection.Create).ToCamelCase()}";
        var eventTypePath = $"{nameof(CloudEventTriggerDefinition.Event).ToCamelCase()}.{nameof(CloudEventFilterDefinition.Type).ToCamelCase()}";
        var eventSourcePath = $"{nameof(CloudEventTriggerDefinition.Event).ToCamelCase()}.{nameof(CloudEventFilterDefinition.Source).ToCamelCase()}";
        var typeFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Regex(eventTypePath, new BsonRegularExpression(command.Event.Type, "i")));
        var sourceFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Or(filterBuilder.Regex(eventSourcePath, new BsonRegularExpression(command.Event.Source.OriginalString, "i")), filterBuilder.Exists(eventSourcePath, false)));
        var finalFilter = filterBuilder.And(typeFilter, sourceFilter);
        var projectionTypes = await (await ProjectionTypes.FindAsync(finalFilter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);

        var x = await ProjectionTypes.FindAsync(Builders<ProjectionType>.Filter.Empty, new(), cancellationToken);

        foreach (var projectionType in projectionTypes)
        {
            var trigger = projectionType.Triggers.Create.FirstOrDefault(t => Regex.IsMatch(command.Event.Type, t.Event.Type) && (string.IsNullOrWhiteSpace(t.Event.Source) || Regex.IsMatch(command.Event.Source.OriginalString, t.Event.Source)));
            if (trigger == null) continue;
            var projections = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(projectionType.Name));
            var projection = (await ExpressionEvaluator.EvaluateAsync<JObject>(trigger.State, command.Event, cancellationToken: cancellationToken).ConfigureAwait(false))!;
            var correlationId = await CloudEventValueResolver.ResolveAsync(trigger.Event.Correlation, command.Event, cancellationToken).ConfigureAwait(false);
            if (projectionType.Relationships != null)
            {
                foreach (var relationship in projectionType.Relationships)
                {
                    var path = relationship.Key;
                    if (!path.StartsWith("$.")) path = $"$.{path}";
                    var key = projection.SelectToken(path);
                    if (key == null) continue;
                    var targetCollection = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(relationship.Target));
                    var target = await (await targetCollection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(key)), new FindOptions<BsonDocument, BsonDocument>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                    if (target == null) continue;
                    var targetValue = JToken.Parse(target.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
                    key.Replace(targetValue);
                }
            }
            var document = BsonDocument.Create(projection);
            document["_id"] = correlationId;
            await projections.InsertOneAsync(document, new InsertOneOptions(), cancellationToken).ConfigureAwait(false);
        }
        return this.Ok();
    }

}
