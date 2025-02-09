namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Create;

/// <summary>
/// Represents the store the the create projection type view
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
public class CreateProjectionTypeStore(ICloudShapesApiClient cloudShapesApi)
    : ComponentStore<CreateProjectionTypeState>(new())
{

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.ProjectionType"/> changes
    /// </summary>
    public IObservable<ProjectionType> ProjectionType => this.Select(state => state.ProjectionType).DistinctUntilChanged();

    /// <summary>
    /// Sets the <see cref="ProjectionType"/>'s name
    /// </summary>
    /// <param name="name">The <see cref="ProjectionType"/>'s name</param>
    public void SetProjectionTypeName(string? name)
    {
        Reduce(state => state with
        {
            ProjectionType = state.ProjectionType with
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
            ProjectionType = state.ProjectionType with
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
            ProjectionType = state.ProjectionType with
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
            ProjectionType = state.ProjectionType with
            {
                Tags = new EquatableDictionary<string, string>(Get().ProjectionType.Tags ?? [])
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
        var tags = Get().ProjectionType.Tags;
        if (tags == null) return;
        tags = [..tags];
        tags.Remove(key);
        if (tags.Count == 0) tags = null;
        Reduce(state => state with
        {
            ProjectionType = state.ProjectionType with
            {
                Tags = tags
            }
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
            ProjectionType = state.ProjectionType with
            {
                Triggers = state.ProjectionType.Triggers with
                {
                    Create = new(state.ProjectionType.Triggers.Create)
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
        var triggers = Get().ProjectionType.Triggers.Create;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        Reduce(state => state with
        {
            ProjectionType = state.ProjectionType with
            {
                Triggers = state.ProjectionType.Triggers with
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
            ProjectionType = state.ProjectionType with
            {
                Triggers = state.ProjectionType.Triggers with
                {
                    Update = new(state.ProjectionType.Triggers.Update ?? [])
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
        var triggers = Get().ProjectionType.Triggers.Update;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        if (triggers.Count == 0) triggers = null;
        Reduce(state => state with
        {
            ProjectionType = state.ProjectionType with
            {
                Triggers = state.ProjectionType.Triggers with
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
            ProjectionType = state.ProjectionType with
            {
                Triggers = state.ProjectionType.Triggers with
                {
                    Delete = new(state.ProjectionType.Triggers.Delete ?? [])
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
        var triggers = Get().ProjectionType.Triggers.Delete;
        triggers = [.. triggers];
        triggers.Remove(trigger);
        if (triggers.Count == 0) triggers = null;
        Reduce(state => state with
        {
            ProjectionType = state.ProjectionType with
            {
                Triggers = state.ProjectionType.Triggers with
                {
                    Delete = triggers
                }
            }
        });
    }

}