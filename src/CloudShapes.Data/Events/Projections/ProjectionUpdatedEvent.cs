namespace CloudShapes.Data.Events.Projections;

/// <summary>
/// Represents the payload of a <see cref="CloudEvent"/> published whenever a projection has been updated
/// </summary>
/// <param name="Id">The id of the updated projection</param>
/// <param name="Type">The name of the updated projection's type</param>
/// <param name="State">The projection's updated state</param>
public record ProjectionUpdatedEvent(string Id, string Type, object State);
