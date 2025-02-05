namespace CloudShapes.Core.Resources;

/// <summary>
/// Represents a projection type
/// </summary>
public record ProjectionType
{

    /// <summary>
    /// Initializes a new <see cref="ProjectionType"/>
    /// </summary>
    public ProjectionType() { }

    /// <summary>
    /// Initializes a new <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="state">The schema that defines, documents and validates the state of projections of this type</param>
    /// <param name="triggers">A list containing the triggers responsible for creating new projections when specific CloudEvents occur</param>
    /// <param name="indexes">A list containing the indexes, if any, of projections of this type</param>
    /// <param name="relationships">list containing the relationships, if any, of projections of this type</param>
    public ProjectionType(string name, JSchema state, ProjectionTriggerCollection triggers, IEnumerable<ProjectionIndexDefinition>? indexes = null, IEnumerable<ProjectionRelationshipDefinition>? relationships = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(triggers);
        Name = name;
        State = state;
        Triggers = triggers;
        Indexes = indexes == null ? null : new(indexes);
        Relationships = relationships == null ? null : new(relationships);
    }

    /// <summary>
    /// Gets/sets the projection type's name
    /// </summary>
    [BsonId]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the schema that defines, documents and validates the state of projections of this type
    /// </summary>
    public virtual JSchema State { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    public virtual ProjectionTriggerCollection Triggers { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the indexes, if any, of projections of this type
    /// </summary>
    public virtual EquatableList<ProjectionIndexDefinition>? Indexes { get; set; }

    /// <summary>
    /// Gets/sets a list containing the relationships, if any, of projections of this type
    /// </summary>
    public virtual EquatableList<ProjectionRelationshipDefinition>? Relationships { get; set; }

}
