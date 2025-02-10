using CloudShapes.Application.Commands.ProjectionTypes;

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Create;

/// <summary>
/// Represents the store the the create projection type view
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="monacoEditorHelper">The service used to to facilitate the Monaco editor configuration</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="yamlSerializer">The service used to serialize/deserialize data to/from YAML</param>
public class CreateProjectionTypeStore(ICloudShapesApiClient cloudShapesApi, IMonacoEditorHelper monacoEditorHelper, IJsonSerializer jsonSerializer, IYamlSerializer yamlSerializer)
    : ComponentStore<CreateProjectionTypeState>(new())
{

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="CreateProjectionTypeState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="CreateProjectionTypeState.Command"/> changes
    /// </summary>
    public IObservable<CreateProjectionTypeCommand> Command => this.Select(state => state.Command).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="CreateProjectionTypeState.Errors"/> changes
    /// </summary>
    public IObservable<EquatableList<string>> Errors => this.Select(state => state.Errors).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe changes to the input of the component used to edit the projection type's schema
    /// </summary>
    protected Subject<string> SchemaEditorValue { get; } = new Subject<string>();

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        this.SchemaEditorValue
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged()
            .Subscribe(text => SetSchema(text));
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s name
    /// </summary>
    /// <param name="name">The <see cref="ProjectionType"/>'s name</param>
    public void SetProjectionTypeName(string? name)
    {
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Name = name!
            }
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s summary
    /// </summary>
    /// <param name="name">The <see cref="ProjectionType"/>'s summary</param>
    public void SetProjectionTypeSummary(string? summary)
    {
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Summary = summary
            }
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s description
    /// </summary>
    /// <param name="name">The <see cref="ProjectionType"/>'s description</param>
    public void SetProjectionTypeDescription(string? description)
    {
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Description = description
            }
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
            Command = state.Command with
            {
                Tags = new EquatableDictionary<string, string>(Get().Command.Tags ?? [])
                {
                    [key] = value
                }
            }
        });
    }

    /// <summary>
    /// Removes a tag from the <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="key">The key of the tag to remove</param>
    public void RemoveTagFromProjectionType(string key)
    {
        var tags = Get().Command.Tags;
        if (tags == null) return;
        tags = [..tags];
        tags.Remove(key);
        if (tags.Count == 0) tags = null;
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Tags = tags
            }
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s raw schema
    /// </summary>
    public void SetSchemaContent(string content)
    {
        SchemaEditorValue.OnNext(content);
    }

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s schema
    /// </summary>
    /// <param name="content">The <see cref="ProjectionType"/>'s content</param>
    protected void SetSchema(string content)
    {
        try
        {
            var serializer = monacoEditorHelper.PreferredLanguage == PreferredLanguage.JSON ? (ITextSerializer)jsonSerializer : yamlSerializer;
            var schema = string.IsNullOrWhiteSpace(content)
                ? new JsonSchemaBuilder().Type(SchemaValueType.Object).Build()
                : serializer.Deserialize<JsonSchema>(content)!;
            Reduce(state => state with
            {
                Command = state.Command with
                {
                    Schema = schema
                }
            });
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Adds the specified <see cref="CloudEventCreateTriggerDefinition"/>
    /// </summary>
    /// <param name="trigger">The <see cref="CloudEventCreateTriggerDefinition"/> to add</param>
    public void AddTrigger(CloudEventCreateTriggerDefinition trigger)
    {
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Triggers = state.Command.Triggers with
                {
                    Create = new(state.Command.Triggers.Create)
                    {
                        trigger
                    }
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
        var triggers = Get().Command.Triggers.Create;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Triggers = state.Command.Triggers with
                {
                    Create = triggers
                }
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
            Command = state.Command with
            {
                Triggers = state.Command.Triggers with
                {
                    Update = new(state.Command.Triggers.Update ?? [])
                    {
                        trigger
                    }
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
        var triggers = Get().Command.Triggers.Update;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        if (triggers.Count == 0) triggers = null;
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Triggers = state.Command.Triggers with
                {
                    Update = triggers
                }
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
            Command = state.Command with
            {
                Triggers = state.Command.Triggers with
                {
                    Delete = new(state.Command.Triggers.Delete ?? [])
                    {
                        trigger
                    }
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
        var triggers = Get().Command.Triggers.Delete;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        if (triggers.Count == 0) triggers = null;
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Triggers = state.Command.Triggers with
                {
                    Delete = triggers
                }
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
            Command = state.Command with
            {
                Relationships = new(state.Command.Relationships ?? [])
                {
                    relationship
                }
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="ProjectionRelationshipDefinition"/>
    /// </summary>
    /// <param name="relationship">The <see cref="ProjectionRelationshipDefinition"/> to remove</param>
    public void RemoveRelationship(ProjectionRelationshipDefinition relationship)
    {
        var relationships = Get().Command.Relationships ?? [];
        relationships = [.. relationships];
        relationships.Remove(relationship);
        if (relationships.Count == 0) relationships = null;
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Relationships = relationships
            }
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
            Command = state.Command with
            {
                Indexes = new(state.Command.Indexes ?? [])
                {
                    index
                }
            }
        });
    }

    /// <summary>
    /// Removes the specified <see cref="ProjectionIndexDefinition"/>
    /// </summary>
    /// <param name="index">The <see cref="ProjectionIndexDefinition"/> to remove</param>
    public void RemoveIndex(ProjectionIndexDefinition index)
    {
        var indexes = Get().Command.Indexes ?? [];
        indexes = [.. indexes];
        indexes.Remove(index);
        if (indexes.Count == 0) indexes = null;
        Reduce(state => state with
        {
            Command = state.Command with
            {
                Indexes = indexes
            }
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

    /// <summary>
    /// Creates the <see cref="ProjectionType"/>
    /// </summary>
    /// <returns>A boolean indicating whether or not the <see cref="ProjectionType"/> has been successfully created</returns>
    public async Task<bool> CreateProjectionTypeAsync()
    {
        try
        {
            await cloudShapesApi.ProjectionTypes.CreateAsync(Get().Command, CancellationTokenSource.Token);
            return true;
        }
        catch (Exception ex)
        {
            AddError(ex.Message);
            return false;
        }
    }

}