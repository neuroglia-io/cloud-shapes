using CloudShapes.Application.Services;
using CloudShapes.Data.Models;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Neuroglia.Data.PatchModel.Services;
using Neuroglia.Serialization;
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
/// <param name="patchHandlers">An <see cref="IEnumerable{T}"/> containing all registered <see cref="IPatchHandler"/>s</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
public class IngestCloudEventCommandHandler(IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IPluralize pluralize, IExpressionEvaluator expressionEvaluator, ICloudEventValueResolver cloudEventValueResolver, IEnumerable<IPatchHandler> patchHandlers, IJsonSerializer jsonSerializer)
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

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all registered <see cref="IPatchHandler"/>s
    /// </summary>
    protected IEnumerable<IPatchHandler> PatchHandlers { get; } = patchHandlers;

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <inheritdoc/>
    public virtual async Task<IOperationResult> HandleAsync(IngestCloudEventCommand command, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<ProjectionType>.Filter;
        var triggerPath = $"{nameof(ProjectionType.Triggers).ToCamelCase()}.{nameof(ProjectionTriggerCollection.Create).ToCamelCase()}";
        var eventTypePath = $"{nameof(CloudEventTriggerDefinition.Event).ToCamelCase()}.{nameof(CloudEventFilterDefinition.Type).ToCamelCase()}";
        var eventSourcePath = $"{nameof(CloudEventTriggerDefinition.Event).ToCamelCase()}.{nameof(CloudEventFilterDefinition.Source).ToCamelCase()}";
        var typeFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Regex(eventTypePath, new BsonRegularExpression(command.Event.Type, "i")));
        var sourceFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Or(filterBuilder.Regex(eventSourcePath, new BsonRegularExpression(command.Event.Source.OriginalString, "i")), filterBuilder.Exists(eventSourcePath, false), filterBuilder.Eq(eventSourcePath, BsonNull.Value)));
        var createFilter = filterBuilder.And(typeFilter, sourceFilter);
        triggerPath = $"{nameof(ProjectionType.Triggers).ToCamelCase()}.{nameof(ProjectionTriggerCollection.Update).ToCamelCase()}";
        typeFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Regex(eventTypePath, new BsonRegularExpression(command.Event.Type, "i")));
        sourceFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Or(filterBuilder.Regex(eventSourcePath, new BsonRegularExpression(command.Event.Source.OriginalString, "i")), filterBuilder.Exists(eventSourcePath, false), filterBuilder.Eq(eventSourcePath, BsonNull.Value)));
        var updateFilter = filterBuilder.And(typeFilter, sourceFilter);
        triggerPath = $"{nameof(ProjectionType.Triggers).ToCamelCase()}.{nameof(ProjectionTriggerCollection.Delete).ToCamelCase()}";
        typeFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Regex(eventTypePath, new BsonRegularExpression(command.Event.Type, "i")));
        sourceFilter = filterBuilder.ElemMatch(triggerPath, filterBuilder.Or(filterBuilder.Regex(eventSourcePath, new BsonRegularExpression(command.Event.Source.OriginalString, "i")), filterBuilder.Exists(eventSourcePath, false), filterBuilder.Eq(eventSourcePath, BsonNull.Value)));
        var deleteFilter = filterBuilder.And(typeFilter, sourceFilter);
        var filter = filterBuilder.Or(createFilter, updateFilter, deleteFilter);
        var projectionTypes = await (await ProjectionTypes.FindAsync(filter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        foreach (var projectionType in projectionTypes)
        {
            var trigger = projectionType.Triggers.AsEnumerable().FirstOrDefault(t => Regex.IsMatch(command.Event.Type, t.Event.Type) && (string.IsNullOrWhiteSpace(t.Event.Source) || Regex.IsMatch(command.Event.Source.OriginalString, t.Event.Source)));
            if (trigger == null) continue;
            var correlationId = (await CloudEventValueResolver.ResolveAsync(trigger.Event.Correlation, command.Event, cancellationToken).ConfigureAwait(false))!;
            switch (trigger)
            {
                case CloudEventCreateTriggerDefinition createTrigger:
                    await this.ProcessCreateTriggerAsync(projectionType, createTrigger, command.Event, correlationId, cancellationToken).ConfigureAwait(false);
                    break;
                case CloudEventUpdateTriggerDefinition updateTrigger:
                    await this.ProcessUpdateTriggerAsync(projectionType, updateTrigger, command.Event, correlationId, cancellationToken).ConfigureAwait(false);
                    break;
                case CloudEventDeleteTriggerDefinition deleteTrigger:
                    await this.ProcessDeleteTriggerAsync(projectionType, deleteTrigger, command.Event, correlationId, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }
        return this.Ok();
    }

    /// <summary>
    /// Processes the specified <see cref="CloudEventCreateTriggerDefinition"/>
    /// </summary>
    /// <param name="projectionType">The <see cref="ProjectionType"/> to handle the <see cref="CloudEventCreateTriggerDefinition"/> for</param>
    /// <param name="trigger">The <see cref="CloudEventCreateTriggerDefinition"/> to handle</param>
    /// <param name="e">The triggering <see cref="CloudEvent"/></param>
    /// <param name="correlationId">The triggering <see cref="CloudEvent"/>'s correlation id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProcessCreateTriggerAsync(ProjectionType projectionType, CloudEventCreateTriggerDefinition trigger, CloudEvent e, string correlationId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projectionType);
        ArgumentNullException.ThrowIfNull(trigger);
        ArgumentNullException.ThrowIfNull(e);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        var projections = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(projectionType.Name));
        var projection = (await ExpressionEvaluator.EvaluateAsync(trigger.State, e, cancellationToken: cancellationToken).ConfigureAwait(false))!;
        var document = BsonDocument.Create(projection);
        document["_id"] = correlationId;
        if (projectionType.Relationships != null)
        {
            foreach (var relationship in projectionType.Relationships)
            {
                //todo: apply to all types of relationships
                var path = relationship.Key;
                var key = document.Find(path);
                if (key == null) continue;
                var targetCollection = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(relationship.Target));
                var target = await (await targetCollection.FindAsync(Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(key)), new FindOptions<BsonDocument, BsonDocument>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (target == null) continue;
                document.Replace(path, target);
            }
        }
        await projections.InsertOneAsync(document, new InsertOneOptions(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the specified <see cref="CloudEventUpdateTriggerDefinition"/>
    /// </summary>
    /// <param name="projectionType">The <see cref="ProjectionType"/> to handle the <see cref="CloudEventUpdateTriggerDefinition"/> for</param>
    /// <param name="trigger">The <see cref="CloudEventUpdateTriggerDefinition"/> to handle</param>
    /// <param name="e">The triggering <see cref="CloudEvent"/></param>
    /// <param name="correlationId">The triggering <see cref="CloudEvent"/>'s correlation id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProcessUpdateTriggerAsync(ProjectionType projectionType, CloudEventUpdateTriggerDefinition trigger, CloudEvent e, string correlationId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projectionType);
        ArgumentNullException.ThrowIfNull(trigger);
        ArgumentNullException.ThrowIfNull(e);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        var projections = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(projectionType.Name));
        var projectionFilter = Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(correlationId));
        var projection = await (await projections.FindAsync(projectionFilter, new FindOptions<BsonDocument, BsonDocument>(), cancellationToken).ConfigureAwait(false)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        if (projection == null) return;
        switch (trigger.Strategy)
        {
            case ProjectionUpdateStrategy.Patch:
                if (trigger.Patch == null) throw new NullReferenceException("The 'patch' property must be configured");
                var patchHandler = PatchHandlers.FirstOrDefault(h => h.Supports(trigger.Patch.Type)) ?? throw new NullReferenceException($"Failed to find an handler for the specified patch type '{trigger.Patch.Type}'");
                var toPatch = JsonSerializer.Deserialize<object>(projection.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson }))!;
                var patched = patchHandler.ApplyPatchAsync(trigger.Patch.Document, toPatch, cancellationToken).ConfigureAwait(false);
                projection = BsonDocument.Create(patched);
                break;
            case ProjectionUpdateStrategy.Replace:
                var toUpdate = JsonSerializer.Deserialize<object>(projection.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson }))!;
                var arguments = new Dictionary<string, object>()
                {
                    { RuntimeExpressions.Arguments.State, toUpdate }
                };
                object updated;
                if (trigger.State is string expression) updated = (await ExpressionEvaluator.EvaluateAsync(expression, e, arguments, cancellationToken: cancellationToken).ConfigureAwait(false))!;
                else updated = (await ExpressionEvaluator.EvaluateAsync(trigger.State!, e, arguments, cancellationToken: cancellationToken).ConfigureAwait(false))!;
                projection = BsonDocument.Create(updated);
                projection["_id"] = correlationId;
                break;
            default:
                throw new NotSupportedException($"The specified update strategy '{trigger.Strategy}' is not supported");
        }
        var result = await projections.ReplaceOneAsync(projectionFilter, projection, new ReplaceOptions(), cancellationToken).ConfigureAwait(false);
        if (result.MatchedCount < 1) return;
        var relatedProjectionTypesFilterBuilder = Builders<ProjectionType>.Filter;
        var relatedProjectionTypesFilter = relatedProjectionTypesFilterBuilder.ElemMatch("relationships", relatedProjectionTypesFilterBuilder.Eq("target", projectionType.Name));
        var relatedProjectionTypes = await (await ProjectionTypes.FindAsync(relatedProjectionTypesFilter, new FindOptions<ProjectionType, ProjectionType>(), cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken).ConfigureAwait(false);
        foreach (var relatedProjectionType in relatedProjectionTypes)
        {
            //todo: apply to all types of relationships
            var relatedProjections = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(relatedProjectionType.Name));
            foreach(var relationship in relatedProjectionType.Relationships!.Where(r => r.Target == projectionType.Name))
            {
                var relatedProjectionsFilterBuilder = Builders<BsonDocument>.Filter;
                var relatedProjectionsFilter = relatedProjectionsFilterBuilder.Eq($"{relationship.Key}._id", correlationId);
                foreach (var relatedProjection in await (await relatedProjections.FindAsync(relatedProjectionsFilter, new FindOptions<BsonDocument, BsonDocument>(), cancellationToken)).ToListAsync(cancellationToken).ConfigureAwait(false))
                {
                    relatedProjection.Replace(relationship.Key, projection);
                    await relatedProjections.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", relatedProjection["_id"]), relatedProjection, new ReplaceOptions(), cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// Processes the specified <see cref="CloudEventDeleteTriggerDefinition"/>
    /// </summary>
    /// <param name="projectionType">The <see cref="ProjectionType"/> to handle the <see cref="CloudEventDeleteTriggerDefinition"/> for</param>
    /// <param name="trigger">The <see cref="CloudEventDeleteTriggerDefinition"/> to handle</param>
    /// <param name="e">The triggering <see cref="CloudEvent"/></param>
    /// <param name="correlationId">The triggering <see cref="CloudEvent"/>'s correlation id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProcessDeleteTriggerAsync(ProjectionType projectionType, CloudEventDeleteTriggerDefinition trigger, CloudEvent e, string correlationId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(projectionType);
        ArgumentNullException.ThrowIfNull(trigger);
        ArgumentNullException.ThrowIfNull(e);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        var projections = Database.GetCollection<BsonDocument>(Pluralize.Pluralize(projectionType.Name));
        await projections.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("_id", BsonValue.Create(correlationId)), new DeleteOptions(), cancellationToken).ConfigureAwait(false);
        //todo: cascading delete
    }

}
