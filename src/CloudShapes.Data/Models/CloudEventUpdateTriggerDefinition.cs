namespace CloudShapes.Data.Models;

/// <summary>
/// Represents a trigger that updates an existing projection when a CloudEvent occurs
/// </summary>
public record CloudEventUpdateTriggerDefinition 
    : CloudEventTriggerDefinition
{

    /// <summary>
    /// Gets/sets an object that represents the state that replaces the current state of the projection<para></para>
    /// Required if strategy is set to <see cref="ProjectionUpdateStrategy.Replace"/>, otherwise ignored<para></para>
    /// Supports runtime expressions
    /// </summary>
    public virtual object? State { get; set; }

    /// <summary>
    /// Gets/sets the patch used to update the current state of the projection<para></para>
    /// Required if strategy is set to <see cref="ProjectionUpdateStrategy.Patch"/>, otherwise ignored<para></para>
    /// Supports runtime expressions
    /// </summary>
    public virtual Patch? Patch { get; set; }

    /// <summary>
    /// Gets the update strategy to use
    /// </summary>
    
    public virtual string Strategy => this.State == null ? this.Patch == null ? throw new Exception("Either 'state' or 'patch' must be configured") : ProjectionUpdateStrategy.Patch : ProjectionUpdateStrategy.Replace;

    /// <summary>
    /// Creates a new <see cref="CloudEventUpdateTriggerDefinition"/> used to replace the current state of the projection it applies to
    /// </summary>
    /// <param name="e">An object used to configure triggering CloudEvents</param>
    /// <param name="state">A key/value mapping of the state that replaces the current state of the projection</param>
    /// <returns>A new <see cref="CloudEventUpdateTriggerDefinition"/></returns>
    public static CloudEventUpdateTriggerDefinition FromState(CloudEventFilterDefinition e, EquatableDictionary<string, object> state) => new() { Event = e, State = state };

    /// <summary>
    /// Creates a new <see cref="CloudEventUpdateTriggerDefinition"/> used to patch the current state of the projection it applies to
    /// </summary>
    /// <param name="e">An object used to configure triggering CloudEvents</param>
    /// <param name="patch">The patch used to update the current state of the projection</param>
    /// <returns>A new <see cref="CloudEventUpdateTriggerDefinition"/></returns>
    public static CloudEventUpdateTriggerDefinition FromPatch(CloudEventFilterDefinition e, Patch patch) => new() { Event = e, Patch = patch };


}
