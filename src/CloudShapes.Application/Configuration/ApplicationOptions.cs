namespace CloudShapes.Application.Configuration;

/// <summary>
/// Represents the options used to configure the application
/// </summary>
public class ApplicationOptions
{

    /// <summary>
    /// Gets/sets the options used to configure the application's database
    /// </summary>
    public virtual DatabaseOptions Database { get; set; } = new();

    /// <summary>
    /// Gets/sets the options used to configure the application's <see cref="CloudEvent"/>s
    /// </summary>
    public virtual CloudEventOptions Events { get; set; } = new();

}
