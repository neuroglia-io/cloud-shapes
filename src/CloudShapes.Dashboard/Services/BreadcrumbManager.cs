namespace CloudShapes.Dashboard.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IBreadcrumbManager"/> interface
/// </summary>
/// <param name="navigationManager">The service used for managing navigation with the current application</param>
public class BreadcrumbManager(NavigationManager navigationManager)
    : IBreadcrumbManager
{

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    List<Components.BreadcrumbItem> _items = [];

    /// <summary>
    /// Gets the service used for managing navigation with the current application
    /// </summary>
    protected NavigationManager NavigationManager { get; } = navigationManager;

    /// <inheritdoc/>
    public IReadOnlyCollection<Components.BreadcrumbItem> Items => this._items;

    /// <inheritdoc/>
    public virtual Components.BreadcrumbItem Add(Components.BreadcrumbItem breadcrumbItem)
    {
        ArgumentNullException.ThrowIfNull(breadcrumbItem);
        this._items.Add(breadcrumbItem);
        this.NotifyChange();
        return breadcrumbItem;
    }

    /// <inheritdoc/>
    public virtual Components.BreadcrumbItem Add(string label, string link, string? icon = null) => this.Add(new(label, link, icon));

    /// <inheritdoc/>
    public virtual void Remove(Components.BreadcrumbItem breadcrumbItem)
    {
        ArgumentNullException.ThrowIfNull(breadcrumbItem);
        this._items.Remove(breadcrumbItem);
        this.NotifyChange();
    }

    /// <inheritdoc/>
    public virtual void Clear()
    {
        this._items.Clear();
        this.NotifyChange();
    }

    /// <inheritdoc/>
    public virtual void Use(params Components.BreadcrumbItem[] breadcrumbs)
    {
        this._items = [.. breadcrumbs];
        this.NotifyChange();
    }

    /// <inheritdoc/>
    public virtual void NavigateTo(Components.BreadcrumbItem breadcrumbItem)
    {
        ArgumentNullException.ThrowIfNull(breadcrumbItem);
        var itemIndex = this._items.IndexOf(breadcrumbItem);
        var breadcrumbItems = this._items.Take(itemIndex + 1).ToArray();
        this.Use(breadcrumbItems);
        if (breadcrumbItem.Link != null)
        {
            this.NavigationManager.NavigateTo(breadcrumbItem.Link);
        }
    }

    /// <summary>
    /// Notifies listeners that the breadcrumbs have changed
    /// </summary>
    protected void NotifyChange() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));

}
