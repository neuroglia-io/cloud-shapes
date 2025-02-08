namespace CloudShapes.Data.Events.Projections;

/// <summary>
/// Represents the payload of a <see cref="CloudEvent"/> published whenever a new projection has been created
/// </summary>
/// <param name="Id">The id of the newly created projection</param>
/// <param name="Type">The name of the newly created projection's type</param>
/// <param name="State">The newly created projection's initial state</param>
public record ProjectionCreatedEvent(string Id, string Type, object State);
