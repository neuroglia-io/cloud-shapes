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

using CloudShapes.Dashboard.Components.ProjectionDetailsStateManagement;
using CloudShapes.Data.Models;
using CloudShapes.Integration.Commands.ProjectionTypes;

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Editor;

/// <summary>
/// Represents the store the the create projection type view
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="monacoEditorHelper">The service used to to facilitate the Monaco editor configuration</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="yamlSerializer">The service used to serialize/deserialize data to/from YAML</param>
public class ProjectionTypeEditorStore(ILogger<ProjectionTypeEditorStore> logger, ICloudShapesApiClient cloudShapesApi, IMonacoEditorHelper monacoEditorHelper, IJsonSerializer jsonSerializer, IYamlSerializer yamlSerializer)
    : ComponentStore<ProjectionTypeEditorState>(new())
{

    bool _disposed;
    readonly Subject<System.Reactive.Unit> _send = new();
    readonly Subject<System.Reactive.Unit> _sent = new();

    #region Selectors
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.ProjectionTypeName"/> changes
    /// </summary>
    public IObservable<string?> ProjectionTypeName => this.Select(state => state.ProjectionTypeName).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Name"/> changes
    /// </summary>
    public IObservable<string?> Name => this.Select(state => state.Name).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Summary"/> changes
    /// </summary>
    public IObservable<string?> Summary => this.Select(state => state.Summary).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Description"/> changes
    /// </summary>
    public IObservable<string?> Description => this.Select(state => state.Description).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Schema"/> changes
    /// </summary>
    public IObservable<JsonSchema?> OriginalSchema => this.Select(state => state.Schema).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.SerializedSchema"/> changes
    /// </summary>
    public IObservable<string?> SerializedSchema => this.Select(state => state.SerializedSchema).DistinctUntilChanged();
    
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.MigrationPatchType"/> changes
    /// </summary>
    public IObservable<string?> MigrationPatchType => this.Select(state => state.MigrationPatchType).DistinctUntilChanged();
    
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.SerializedMigration"/> changes
    /// </summary>
    public IObservable<string?> SerializedMigration => this.Select(state => state.SerializedMigration).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.ValidateMigration"/> changes
    /// </summary>
    public IObservable<bool> ValidateMigration => this.Select(state => state.ValidateMigration).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Triggers"/> changes
    /// </summary>
    public IObservable<ProjectionTriggerCollection> Triggers => this.Select(state => state.Triggers).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Indexes"/> changes
    /// </summary>
    public IObservable<EquatableList<ProjectionIndexDefinition>> Indexes => this.Select(state => state.Indexes).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Relationships"/> changes
    /// </summary>
    public IObservable<EquatableList<ProjectionRelationshipDefinition>> Relationships => this.Select(state => state.Relationships).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Tags"/> changes
    /// </summary>
    public IObservable<EquatableDictionary<string, string>> Tags => this.Select(state => state.Tags).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeEditorState.Errors"/> changes
    /// </summary>
    public IObservable<EquatableList<string>> Errors => this.Select(state => state.Errors).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe whenever the <see cref="ProjectionType"/> has been successfully sent to the server
    /// </summary>
    public IObservable<System.Reactive.Unit> Sent => _sent.AsObservable();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe the deserialized  <see cref="ProjectionType"/>'s schema
    /// </summary>
    public IObservable<JsonSchema?> Schema => SerializedSchema.Select(content => {
        try { 
            var serializer = monacoEditorHelper.PreferredLanguage == PreferredLanguage.JSON ? (ITextSerializer)jsonSerializer : yamlSerializer;
            var schema = string.IsNullOrWhiteSpace(content)
                ? new JsonSchemaBuilder().Type(SchemaValueType.Object).Build()
                : serializer.Deserialize<JsonSchema>(content)!;
            return schema;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to deserialize schema: {ex.ToString()}");
            return null;
        }
    }).DistinctUntilChanged();

    /// <summary>
    ///  Gets an <see cref="IObservable{T}"/> used to observe a derived <see cref="ProjectionType"/> to edit
    /// </summary>
    public IObservable<ProjectionType> ProjectionType => Observable.CombineLatest(
        Name.Where(name => !string.IsNullOrEmpty(name)),
        Summary,
        Description,
        Schema.Where(schema => schema != null),
        Triggers,
        Indexes,
        Relationships,
        Tags,
        (name, summary, description, schema, triggers, indexes, relationships, tags) => new ProjectionType(name!, schema!, triggers, indexes, relationships, summary, description, tags)
    ).DistinctUntilChanged();

    /// <summary>
    /// Gets the <see cref="IObservable{T}"/>  used to observe a derived <see cref="Patch"/> to migrate the <see cref="ProjectionType"/>'s schema
    /// </summary>
    public IObservable<Patch?> Migration => Observable.CombineLatest(
        MigrationPatchType.Where(type => !string.IsNullOrWhiteSpace(type)),
        SerializedMigration.Where(migration => !string.IsNullOrWhiteSpace(migration)),
        (type, migration) =>
        {
            try { 
                var serializer = monacoEditorHelper.PreferredLanguage == PreferredLanguage.JSON ? (ITextSerializer)jsonSerializer : yamlSerializer;
                var document = serializer.Deserialize<object>(migration!)!;
                return new Patch(type!, document);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to deserialize migration: {ex.ToString()}");
                return null;
            }
        }
    ).DistinctUntilChanged();
    #endregion

    #region Setters
    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeEditorState.OriginalProjectionType"/>
    /// </summary>
    /// <param name="projectionType">The new value</param>
    protected void SetOriginalProjectionType(ProjectionType projectionType)
    {
        Reduce(state => state with
        {
            OriginalProjectionType = projectionType
        });
    }

    /// <summary>
    /// Sets the state's properties derived from the specified <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="projectionType">The <see cref="ProjectionType"/> to set the properties from</param>
    protected void SetProjectionTypeProperties(ProjectionType projectionType)
    {
        Reduce(state => state with
        {
            Name = projectionType.Name,
            Summary = projectionType.Summary,
            Description = projectionType.Description,
            Schema = projectionType.Schema,
            Triggers = projectionType.Triggers,
            Indexes = projectionType.Indexes ?? [],
            Relationships = projectionType.Relationships ?? [],
            Tags = projectionType.Tags ?? []
        });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeEditorState.ProjectionTypeName"/> and <see cref="ProjectionTypeEditorState.Name"/>
    /// </summary>
    /// <param name="projectionTypeName">The <see cref="ProjectionType"/>'s name</param>
    public void SetProjectionTypeName(string? projectionTypeName)
    {
        Reduce(state => state with
        {
            ProjectionTypeName = projectionTypeName,
            Name = projectionTypeName
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s name
    /// </summary>
    /// <param name="name">The <see cref="ProjectionType"/>'s name</param>
    public void SetName(string? name)
    {
        Reduce(state => state with
        {
            Name = name
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s summary
    /// </summary>
    /// <param name="summary">The <see cref="ProjectionType"/>'s summary</param>
    public void SetProjectionTypeSummary(string? summary)
    {
        Reduce(state => state with
        {
            Summary = summary
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s description
    /// </summary>
    /// <param name="description">The <see cref="ProjectionType"/>'s description</param>
    public void SetProjectionTypeDescription(string? description)
    {
        Reduce(state => state with
        {
            Description = description
        });
    }

    /// <summary>
    /// Adds a new tag to the <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="key">The key of the tag to add</param>
    /// <param name="value">The value of the tag to add</param>
    public void AddTagToProjectionType(string key, string value)
    {
        Reduce(state => state with
        {
            Tags = new EquatableDictionary<string, string>(Get().Tags ?? [])
            {
                [key] = value
            }
        });
    }

    /// <summary>
    /// Removes a tag from the <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="key">The key of the tag to remove</param>
    public void RemoveTagFromProjectionType(string key)
    {
        var tags = Get().Tags;
        tags = [.. tags];
        tags.Remove(key);
        Reduce(state => state with
        {
            Tags = tags
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s serialized schema
    /// </summary>
    /// <param name="serializedSchema">The serialized schema</param>
    public void SetSerializedSchema(string? serializedSchema)
    {
        Reduce(state => state with
        {
            SerializedSchema = serializedSchema
        });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeEditorState.MigrationPatchType"/>
    /// </summary>
    /// <param name="migrationPatchType">The new value</param>
    public void SetMigrationPatchType(string? migrationPatchType)
    {
        Reduce(state => state with
        {
            MigrationPatchType = migrationPatchType
        });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeEditorState.SerializedMigration"/>
    /// </summary>
    /// <param name="SerializedMigration">The new value</param>
    public void SetSerializedMigration(string? SerializedMigration)
    {
        Reduce(state => state with
        {
            SerializedMigration = SerializedMigration
        });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeEditorState.ValidateMigration"/>
    /// </summary>
    /// <param name="validateMigration">The new value</param>
    public void SetValidateMigration(bool validateMigration)
    {
        Reduce(state => state with
        {
            ValidateMigration = validateMigration
        });
    }

    /// <summary>
    /// Adds the specified <see cref="CloudEventCreateTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventCreateTriggerDefinition"/> to add</param>
    public void AddTrigger(CloudEventCreateTriggerDefinition trigger)
    {
        Reduce(state => state with
        {
            Triggers = state.Triggers with
            {
                Create = new(state.Triggers.Create)
                {
                    trigger
                }
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="CloudEventCreateTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventCreateTriggerDefinition"/> to remove</param>
    public void RemoveCreateTrigger(CloudEventCreateTriggerDefinition trigger)
    {
        var triggers = Get().Triggers.Create;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        Reduce(state => state with
        {
            Triggers = state.Triggers with
            {
                Create = triggers
            }
        });
    }

    /// <summary>
    /// Adds the specified <see cref="CloudEventUpdateTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventUpdateTriggerDefinition"/> to add</param>
    public void AddTrigger(CloudEventUpdateTriggerDefinition trigger)
    {
        Reduce(state => state with
        {
            Triggers = state.Triggers with
            {
                Update = new(state.Triggers.Update ?? [])
                {
                    trigger
                }
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="CloudEventUpdateTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventUpdateTriggerDefinition"/> to remove</param>
    public void RemoveUpdateTrigger(CloudEventUpdateTriggerDefinition trigger)
    {
        var triggers = Get().Triggers.Update;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        Reduce(state => state with
        {
            Triggers = state.Triggers with
            {
                Update = triggers
            }
        });
    }

    /// <summary>
    /// Adds the specified <see cref="CloudEventDeleteTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventDeleteTriggerDefinition"/> to add</param>
    public void AddTrigger(CloudEventDeleteTriggerDefinition trigger)
    {
        Reduce(state => state with
        {
            Triggers = state.Triggers with
            {
                Delete = new(state.Triggers.Delete ?? [])
                {
                    trigger
                }
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="CloudEventDeleteTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventDeleteTriggerDefinition"/> to remove</param>
    public void RemoveDeleteTrigger(CloudEventDeleteTriggerDefinition trigger)
    {
        var triggers = Get().Triggers.Delete;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        if (triggers.Count == 0) triggers = null;
        Reduce(state => state with
        {
            Triggers = state.Triggers with
            {
                Delete = triggers
            }
        });
    }

    /// <summary>
    /// Adds the specified <see cref="ProjectionRelationshipDefinition"/>
    /// </summary>
    /// <param name="relationship">The <see cref="ProjectionRelationshipDefinition"/> to add</param>
    public void AddRelationship(ProjectionRelationshipDefinition relationship)
    {
        Reduce(state => state with
        {
            Relationships = new(state.Relationships ?? [])
            {
                relationship
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="ProjectionRelationshipDefinition"/>
    /// </summary>
    /// <param name="relationship">The <see cref="ProjectionRelationshipDefinition"/> to remove</param>
    public void RemoveRelationship(ProjectionRelationshipDefinition relationship)
    {
        var relationships = Get().Relationships ?? [];
        relationships = [.. relationships];
        relationships.Remove(relationship);
        Reduce(state => state with
        {
            Relationships = relationships
        });
    }

    /// <summary>
    /// Adds the specified <see cref="ProjectionIndexDefinition"/>
    /// </summary>
    /// <param name="index">The <see cref="ProjectionIndexDefinition"/> to add</param>
    public void AddIndex(ProjectionIndexDefinition index)
    {
        Reduce(state => state with
        {
            Indexes = new(state.Indexes ?? [])
            {
                index
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="ProjectionIndexDefinition"/>
    /// </summary>
    /// <param name="index">The <see cref="ProjectionIndexDefinition"/> to remove</param>
    public void RemoveIndex(ProjectionIndexDefinition index)
    {
        var indexes = Get().Indexes ?? [];
        indexes = [.. indexes];
        indexes.Remove(index);
        Reduce(state => state with
        {
            Indexes = indexes
        });
    }

    /// <summary>
    /// Adds the specified error
    /// </summary>
    /// <param name="error">The error to add</param>
    public void AddError(string error)
    {
        Reduce(state => state with
        {
            Errors = new(state.Errors)
            {
                error
            }
        });
    }

    /// <summary>
    /// Clears all pending errors
    /// </summary>
    public void ClearErrors()
    {
        Reduce(state => state with
        {
            Errors = []
        });
    }

    #endregion

    #region Actions
    /// <summary>
    /// Sends the current projection type to the server
    /// </summary>
    public void Send()
    {
        _send.OnNext(System.Reactive.Unit.Default);
    }

    /// <summary>
    /// Creates the <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="command">The <see cref="CreateProjectionTypeCommand"/> to send</param>
    /// <returns>A awaitable task</returns>
    public async Task CreateProjectionTypeAsync(CreateProjectionTypeCommand command)
    {
        try
        {
            await cloudShapesApi.ProjectionTypes.CreateAsync(command, CancellationTokenSource.Token);
            _sent.OnNext(System.Reactive.Unit.Default);
        }
        catch (Exception ex)
        {
            AddError(ex.Message);
        }
    }

    /// <summary>
    /// Migrates the <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="command">The <see cref="MigrateProjectionTypeSchemaCommand"/> to send</param>
    /// <returns>A awaitable task</returns>
    public async Task MigrateProjectionTypeAsync(MigrateProjectionTypeSchemaCommand command)
    {
        try
        {
            var ProjectionTypeSchemaMigrationResult = await cloudShapesApi.ProjectionTypes.MigrateSchemaAsync(command, CancellationTokenSource.Token);
            _sent.OnNext(System.Reactive.Unit.Default);
        }
        catch (ProblemDetailsException ex)
        {
            AddError(ex.Message);
        }
    }
    #endregion

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        ProjectionTypeName
            .Where(name => !string.IsNullOrEmpty(name))
            .SubscribeAsync(async name => {
                var projectionType = await cloudShapesApi.ProjectionTypes.GetAsync(name!, CancellationTokenSource.Token);
                SetOriginalProjectionType(projectionType);
                SetProjectionTypeProperties(projectionType);
            }, CancellationTokenSource.Token);
        _send.WithLatestFrom(
            Observable.CombineLatest(
                ProjectionTypeName.Where(name => string.IsNullOrEmpty(name)),
                ProjectionType,
                (_, projectionType) => new CreateProjectionTypeCommand()
                {
                    Name = projectionType.Name,
                    Summary = projectionType.Summary,
                    Description = projectionType.Description,
                    Schema = projectionType.Schema,
                    Triggers = projectionType.Triggers,
                    Indexes = projectionType.Indexes,
                    Relationships = projectionType.Relationships,
                    Tags = projectionType.Tags
                }
            ),
            (_, command) => command
        ).SubscribeAsync(CreateProjectionTypeAsync, CancellationTokenSource.Token);
        _send.WithLatestFrom(
        Observable.CombineLatest(
                ProjectionTypeName.Where(name => !string.IsNullOrEmpty(name)),
                Schema.Where(schema => schema != null),
                Migration,
                ValidateMigration,
                (name, schema, migration, validateMigration) => new MigrateProjectionTypeSchemaCommand()
                {
                    Name = name!,
                    Schema = schema!,
                    Migration = migration,
                    Validate = validateMigration
                }
            ),
            (_, command) => command
        ).SubscribeAsync(MigrateProjectionTypeAsync, CancellationTokenSource.Token);
    }

    /// <summary>
    /// Disposes of the store
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the dispose of the store</param>
    protected override void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._send.OnCompleted();
            }
            this._disposed = true;
        }
    }
}