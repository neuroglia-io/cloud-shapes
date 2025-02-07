using CloudShapes.Data.Models;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace CloudShapes.Dashboard.Pages.Projections.List;

/// <summary>
/// Represents the projection list view's <see cref="ComponentStore{TState}"/>
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="pluralize">The service used to pluralize words</param>
public class ProjectionListStore(ICloudShapesApiClient cloudShapesApi, IPluralize pluralize)
    : ComponentStore<ProjectionListState>(new())
{

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.ProjectionTypeName"/> changes
    /// </summary>
    public IObservable<string?> ProjectionTypeName => this.Select(state => state.ProjectionTypeName).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.ProjectionType"/> changes
    /// </summary>
    public IObservable<ProjectionType?> ProjectionType => this.Select(state => state.ProjectionType).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Projections"/> changes
    /// </summary>
    public IObservable<PagedResult<IDictionary<string, object>>?> Projections => this.Select(state => state.Projections).DistinctUntilChanged();

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        ProjectionTypeName.SubscribeAsync(async typeName =>
        {
            var projectionTypes = Get().ProjectionTypes;
            if (Get().ProjectionTypes.Count < 1 || Get().ProjectionType?.Name == typeName) return;
            SetProjectionType();
            await ListProjectionsAsync();
        }, CancellationTokenSource.Token);
        await ListProjectionTypesAsync();
        await ListProjectionsAsync();
        SetLoading(false);
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.Loading"/>
    /// </summary>
    /// <param name="loading">The new loading state</param>
    public void SetLoading(bool loading)
    {
        Reduce(state => state with
        {
            Loading = loading
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.ProjectionTypeName"/>
    /// </summary>
    /// <param name="typeName">The type of the projections to list</param>
    public void SetProjectionTypeName(string typeName)
    {
        Reduce(state => state with
        {
            ProjectionTypeName = typeName
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.QueryOptions"/>
    /// </summary>
    /// <param name="options">The <see cref="QueryOptions"/> to use</param>
    public void SetQueryOptions(QueryOptions options)
    {
        Reduce(state => state with
        {
            QueryOptions = options
        });
    }

    /// <summary>
    /// Lists available <see cref="ProjectionType"/>s
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task ListProjectionTypesAsync()
    {
        var projectionTypes = (await cloudShapesApi.ProjectionTypes.ListAsync()).Items.OrderBy(p => p.Name).ToList();
        Reduce(state => state with
        {
            ProjectionTypes = new(projectionTypes)
        });
        if (projectionTypes == null || projectionTypes.Count < 1) return;
        SetProjectionType();
    }

    /// <summary>
    /// Lists projections of the specified type
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task ListProjectionsAsync()
    {
        var projectionTypes = Get().ProjectionTypes;
        if (projectionTypes == null || projectionTypes.Count < 1) return;
        var projections = await cloudShapesApi.Projections.ListAsync(Get().ProjectionType!.Name, Get().QueryOptions);
        Reduce(state => state with
        {
            Projections = projections
        });
    }

    /// <summary>
    /// Sets the current projection type
    /// </summary>
    protected void SetProjectionType()
    {
        var projectionTypes = Get().ProjectionTypes;
        var projectionTypeName = Get().ProjectionTypeName;
        if (!string.IsNullOrWhiteSpace(projectionTypeName) && pluralize.IsPlural(projectionTypeName)) projectionTypeName = pluralize.Singularize(projectionTypeName);
        if (!string.IsNullOrWhiteSpace(projectionTypeName) && Get().ProjectionType?.Name == projectionTypeName) return;
        var projectionType = string.IsNullOrWhiteSpace(projectionTypeName) ? projectionTypes.First() : projectionTypes.FirstOrDefault(t => t.Name.Equals(projectionTypeName, StringComparison.OrdinalIgnoreCase));
        Reduce(state => state with
        {
            ProjectionType = projectionType
        });
    }

    /// <summary>
    /// Provides data to the view's sidebar
    /// </summary>
    /// <param name="request">The <see cref="SidebarDataProviderRequest"/> to handle</param>
    /// <returns>A new <see cref="SidebarDataProviderResult"/></returns>
    public Task<SidebarDataProviderResult> ProvideSidebarDataAsync(SidebarDataProviderRequest request)
    {
        var navItems = Get().ProjectionTypes.Select(t =>
        {
            var plural = pluralize.Pluralize(t.Name);
            return new NavItem()
            {
                Href = $"/{plural.ToCamelCase()}",
                IconName = IconName.Cast,
                Text = $"{plural} ({t.Metadata.ProjectionCount})"
            };
        });
        return Task.FromResult(request.ApplyTo(navItems));
    }

    /// <summary>
    /// Provides items to the <see cref="Virtualize{TItem}"/> component
    /// </summary>
    /// <param name="request">The <see cref="ItemsProviderRequest"/> to execute</param>
    /// <returns>The resulting <see cref="ItemsProviderResult{TResult}"/></returns>
    public ValueTask<ItemsProviderResult<IDictionary<string, object>>> ProvideProjectionsAsync(ItemsProviderRequest request)
    {
        var projections = Get().Projections;
        return ValueTask.FromResult(new ItemsProviderResult<IDictionary<string, object>>(projections?.Items ?? [], (int)(projections?.TotalCount ?? 0)));
    }

}