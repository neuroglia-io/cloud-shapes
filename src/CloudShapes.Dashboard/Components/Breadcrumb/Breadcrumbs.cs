namespace CloudShapes.Dashboard.Components;

/// <summary>
/// Exposes the default breadcrumbs for all pages of the application
/// </summary>
public static class Breadcrumbs
{

    /// <summary>
    /// Holds the breadcrumb items for <see cref="Workflow"/> related routes
    /// </summary>
    public static BreadcrumbItem[] Workflows = [new("Workflows", "/workflows")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="WorkflowInstance"/> related routes
    /// </summary>
    public static BreadcrumbItem[] WorkflowInstances = [new("Workflows Instances", "/workflow-instances")];
    /// <summary>
    /// Holds the breadcrumb items for the user profile related routes
    /// </summary>
    public static BreadcrumbItem[] UserProfile = [new("User Profile", "/users/profile")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="Operator"/> related routes
    /// </summary>
    public static BreadcrumbItem[] Operators = [new("Operators", "/operators")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="Namespace"/> related routes
    /// </summary>
    public static BreadcrumbItem[] Namespaces = [new("Namespaces", "/operators")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="CustomFunction"/> related routes
    /// </summary>
    public static BreadcrumbItem[] Functions = [new("Functions", "/functions")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="Correlator"/> related routes
    /// </summary>
    public static BreadcrumbItem[] Correlators = [new("Correlators", "/correlators")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="Correlation"/> related routes
    /// </summary>
    public static BreadcrumbItem[] Correlations = [new("Correlations", "/correlations")];
    /// <summary>
    /// Holds the breadcrumb items for <see cref="ServiceAccount"/> related routes
    /// </summary>
    public static BreadcrumbItem[] ServiceAccounts = [new("Service Accounts", "/service-accounts")];
    /// <summary>
    /// Holds the breadcrumb items for about related routes
    /// </summary>
    public static BreadcrumbItem[] About = [new("About", "/about")];

}
