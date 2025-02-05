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

}
