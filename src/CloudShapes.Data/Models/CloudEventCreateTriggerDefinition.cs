namespace CloudShapes.Data.Models;

/// <summary>
/// Represents a trigger that creates a new projection when a CloudEvent occurs
/// </summary>
public record CloudEventCreateTriggerDefinition 
    : CloudEventTriggerDefinition
{

    /// <summary>
    /// Gets/sets a an object that represents the projection's initial state<para></para>
    /// Supports runtime expressions
    /// </summary>
    public virtual object State { get; set; } = null!;

}