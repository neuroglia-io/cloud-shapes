namespace CloudShapes.Data.Events.Projections;

/// <summary>
/// Represents the payload of a <see cref="CloudEvent"/> published whenever a projection has been deleted
/// </summary>
/// <param name="Id">The id of the deleted projection</param>
/// <param name="Type">The name of the deleted projection's type</param>
public record ProjectionDeletedEvent(string Id, string Type);
