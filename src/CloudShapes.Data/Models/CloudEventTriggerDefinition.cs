namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the definition of a CloudEvent based trigger
/// </summary>
public abstract record CloudEventTriggerDefinition
{

    /// <summary>
    /// Gets/sets an object used to configure triggering CloudEvents
    /// </summary>
    public virtual CloudEventFilterDefinition Event { get; set; } = null!;

}
