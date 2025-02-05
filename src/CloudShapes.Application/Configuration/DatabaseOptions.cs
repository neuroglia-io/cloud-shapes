namespace CloudShapes.Application.Configuration;

/// <summary>
/// Represents the options used to configure the application's database
/// </summary>
public class DatabaseOptions
{

    /// <summary>
    /// Gets/sets the database's connection string
    /// </summary>
    public virtual string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Gets/sets the database's name
    /// </summary>
    public virtual string Name { get; set; } = "CloudShapes";

}
