namespace CloudShapes.Dashboard.Components;

/// <summary>
/// Represents a breadcrumb element
/// </summary>
/// <remarks>
/// Initializes a new <see cref="BreadcrumbItem"/> with the provided data
/// </remarks>
public class BreadcrumbItem
{

    /// <summary>
    /// Initializes a new <see cref="BreadcrumbItem"/> with the provided data
    /// </summary>
    /// <param name="label">The breadcrumb's label</param>
    /// <param name="link">The link associated to the breadcrumb</param>
    /// <param name="icon">The breadcrumb's icon, if any</param>
    public BreadcrumbItem(string label, string link, string? icon = null)
    {
        Label = label;
        Link = link;
        Icon = icon;
    }

    /// <summary>
    /// Initializes a new <see cref="BreadcrumbItem"/> with the provided template
    /// </summary>
    /// <param name="template">The breadcrumb's template</param>
    public BreadcrumbItem(RenderFragment template)
    {
        Template = template;
    }

    /// <summary>
    /// Gets the breadcrumb's label, if any
    /// </summary>
    public string? Label { get; }

    /// <summary>
    /// Gets the link associated to the breadcrumb, if any
    /// </summary>
    public string? Link { get; }

    /// <summary>
    /// Gets the breadcrumb's icon, if any
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    /// Get the breadcrumb's <see cref="RenderFragment"/>, if any
    /// </summary>
    public RenderFragment? Template { get; }

}