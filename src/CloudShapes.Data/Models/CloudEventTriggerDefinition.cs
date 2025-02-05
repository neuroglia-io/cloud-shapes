namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the definition of a CloudEvent based trigger
/// </summary>
public abstract record CloudEventTriggerDefinition
{

    /// <summary>
    /// Gets/sets the trigger's name
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the trigger's description
    /// </summary>
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets/sets an object used to configure triggering CloudEvents
    /// </summary>
    public virtual CloudEventFilterDefinition Event { get; set; } = null!;

}
