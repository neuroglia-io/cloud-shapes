namespace CloudShapes.Data.Events.ProjectionTypes;

/// <summary>
/// Represents the payload of the <see cref="CloudEvent"/> published whenever a <see cref="ProjectionType"/> has been deleted
/// </summary>
/// <param name="Name">The name of the <see cref="ProjectionType"/> that has been deleted</param>
public record ProjectionTypeDeletedEvent(string Name);