namespace CloudShapes.Core.Resources;

/// <summary>
/// Represents a collection of triggers used to create, update and delete projections of a specific type
/// </summary>
public record ProjectionTriggerCollection
{

    /// <summary>
    /// Initializes a new <see cref="ProjectionTriggerCollection"/>
    /// </summary>
    public ProjectionTriggerCollection() { }

    /// <summary>
    /// Initializes a new <see cref="ProjectionTriggerCollection"/>
    /// </summary>
    /// <param name="create">A list containing the triggers responsible for creating new projections when specific CloudEvents occur</param>
    /// <param name="update">A list containing the triggers responsible for updating projections when specific CloudEvents occur</param>
    /// <param name="delete">A list containing the triggers responsible for deleting projections when specific CloudEvents occur</param>
    public ProjectionTriggerCollection(IEnumerable<CloudEventCreateTriggerDefinition> create, IEnumerable<CloudEventUpdateTriggerDefinition>? update = null, IEnumerable<CloudEventDeleteTriggerDefinition>? delete = null)
    {
        ArgumentNullException.ThrowIfNull(create);
        if (!create.Any()) throw new ArgumentOutOfRangeException(nameof(create), "The trigger collection must contain at least one trigger to define how projections are created");
        this.Create = new(create);
        this.Update = update == null ? null : new(update);
        this.Delete = delete == null ? null : new(delete);
    }

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    public virtual EquatableList<CloudEventCreateTriggerDefinition> Create { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for updating projections when specific CloudEvents occur
    /// </summary>
    public virtual EquatableList<CloudEventUpdateTriggerDefinition>? Update { get; set; }

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for deleting projections when specific CloudEvents occur
    /// </summary>
    public virtual EquatableList<CloudEventDeleteTriggerDefinition>? Delete { get; set; }

}