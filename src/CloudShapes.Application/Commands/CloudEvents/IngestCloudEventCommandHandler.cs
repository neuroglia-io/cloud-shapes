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

using CloudShapes.Integration.Commands.CloudEvents;

namespace CloudShapes.Application.Commands.CloudEvents;

/// <summary>
/// Represents the service used to handle <see cref="IngestCloudEventCommand"/>s
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="database">The current <see cref="IMongoDatabase"/></param>
/// <param name="projectionTypes">The <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s</param>
/// <param name="dbContext">The current <see cref="IDbContext"/></param>
/// <param name="expressionEvaluator">The service used to evaluate runtime expressions</param>
/// <param name="cloudEventValueResolver">The service used to resolve values from <see cref="CloudEvent"/>s</param>
/// <param name="patchHandlers">An <see cref="IEnumerable{T}"/> containing all registered <see cref="IPatchHandler"/>s</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="schemaValidator">The service used to validate schemas</param>
public class IngestCloudEventCommandHandler(ILogger<IngestCloudEventCommandHandler> logger, IMongoDatabase database, IMongoCollection<ProjectionType> projectionTypes, IDbContext dbContext, 
    IExpressionEvaluator expressionEvaluator, ICloudEventCorrelationKeyResolver cloudEventValueResolver, IEnumerable<IPatchHandler> patchHandlers, ISchemaValidator schemaValidator, IJsonSerializer jsonSerializer)
    : ICommandHandler<IngestCloudEventCommand>
{

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Gets the current <see cref="IMongoDatabase"/>
    /// </summary>
    protected IMongoDatabase Database { get; } = database;

    /// <summary>
    /// Gets the <see cref="IMongoCollection{TDocument}"/> used to persist <see cref="ProjectionType"/>s
    /// </summary>
    protected IMongoCollection<ProjectionType> ProjectionTypes { get; } = projectionTypes;

    /// <summary>
    /// Gets the current <see cref="IDbContext"/>
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Gets the service used to evaluate runtime expressions
    /// </summary>
    protected IExpressionEvaluator ExpressionEvaluator { get; } = expressionEvaluator;

    /// <summary>
    /// Gets the service used to resolve values from <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventCorrelationKeyResolver CloudEventValueResolver { get; } = cloudEventValueResolver;

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all registered <see cref="IPatchHandler"/>s
    /// </summary>
    protected IEnumerable<IPatchHandler> PatchHandlers { get; } = patchHandlers;

    /// <summary>
    /// Gets the service used to validate schemas
    /// </summary>
    protected ISchemaValidator SchemaValidator { get; } = schemaValidator;

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
        var projection = (await ExpressionEvaluator.EvaluateAsync(trigger.State, e, cancellationToken: cancellationToken).ConfigureAwait(false))!;
        var validationResult = await SchemaValidator.ValidateAsync(projection, projectionType.Schema, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsSuccess()) throw new Exception($"Failed to validate the projection of type '{projectionType.Name}':{Environment.NewLine}{string.Join(Environment.NewLine, validationResult.Errors!)}");
        var document = BsonDocument.Create(projection);
        document["_id"] = correlationId;
        await DbContext.Set(projectionType).AddAsync(document, cancellationToken).ConfigureAwait(false);
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
        var projections = DbContext.Set(projectionType);
        var projection = await projections.GetAsync(correlationId, cancellationToken).ConfigureAwait(false);
        if (projection == null) return;
        var metadata = projection[DocumentMetadata.PropertyName];
        switch (trigger.Strategy)
        {
            case ProjectionUpdateStrategy.Patch:
                if (trigger.Patch == null) throw new NullReferenceException("The 'patch' property must be configured");
                var patchHandler = PatchHandlers.FirstOrDefault(h => h.Supports(trigger.Patch.Type)) ?? throw new NullReferenceException($"Failed to find an handler for the specified patch type '{trigger.Patch.Type}'");
                var toPatch = JsonSerializer.Deserialize<object>(projection.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson }))!;
                var patched = patchHandler.ApplyPatchAsync(trigger.Patch.Document, toPatch, cancellationToken).ConfigureAwait(false);
                var validationResult = await SchemaValidator.ValidateAsync(patched, projectionType.Schema, cancellationToken).ConfigureAwait(false);
                if (!validationResult.IsSuccess()) throw new Exception($"Failed to validate the projection of type '{projectionType.Name}':{Environment.NewLine}{string.Join(Environment.NewLine, validationResult.Errors!)}");
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
                validationResult = await SchemaValidator.ValidateAsync(updated, projectionType.Schema, cancellationToken).ConfigureAwait(false);
                if (!validationResult.IsSuccess()) throw new Exception($"Failed to validate the projection of type '{projectionType.Name}':{Environment.NewLine}{string.Join(Environment.NewLine, validationResult.Errors!)}");
                projection = BsonDocument.Create(updated);
                projection["_id"] = correlationId;
                projection[DocumentMetadata.PropertyName] = metadata;
                break;
            default:
                throw new NotSupportedException($"The specified update strategy '{trigger.Strategy}' is not supported");
        }
        await projections.UpdateAsync(projection, cancellationToken).ConfigureAwait(false);
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
        await DbContext.Set(projectionType).DeleteAsync(correlationId, cancellationToken).ConfigureAwait(false);
        //todo: cascading delete
    }

}
