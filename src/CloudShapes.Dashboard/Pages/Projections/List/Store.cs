using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace CloudShapes.Dashboard.Pages.Projections.List;

/// <summary>
/// Represents the projection list view's <see cref="ComponentStore{TState}"/>
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="cloudEventHub">The service used to listen to <see cref="CloudEvent"/>s produced by Cloud Shapes</param>
/// <param name="pluralize">The service used to pluralize words</param>
public class ProjectionListStore(ICloudShapesApiClient cloudShapesApi, CloudEventHubClient cloudEventHub, IPluralize pluralize)
    : ComponentStore<ProjectionListState>(new())
{

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe the <see cref="CloudEvent"/>s produced by Cloud Shapes
    /// </summary>
    protected IObservable<CloudEvent> CloudEvents { get; private set; } = null!;

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
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.ProjectionTypes"/> changes
    /// </summary>
    public IObservable<EquatableList<ProjectionType>?> ProjectionTypes => this.Select(state => state.ProjectionTypes).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Projections"/> changes
    /// </summary>
    public IObservable<PagedResult<IDictionary<string, object>>?> Projections => this.Select(state => state.Projections).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.SelectedProjections"/> changes
    /// </summary>
    public IObservable<EquatableList<string>> SelectedProjections => this.Select(state => state.SelectedProjections).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.QueryOptions"/> changes
    /// </summary>
    public IObservable<QueryOptions> QueryOptions => this.Select(state => state.QueryOptions).DistinctUntilChanged();

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await cloudEventHub.StartAsync().ConfigureAwait(false);
        CloudEvents = cloudEventHub.Stream();
        CloudEvents.Where(e => CloudShapes.CloudEvents.ProjectionTypes.GetTypes().Contains(e.Type)).SubscribeAsync(async _ => await ListProjectionTypesAsync(), CancellationTokenSource.Token);
        CloudEvents.Where(e => CloudShapes.CloudEvents.Projections.GetTypes().Contains(e.Type) && (string)((IDictionary<string, object>)e.Data!)["type"] == Get().ProjectionType?.Name).SubscribeAsync(OnCloudEventAsync, CancellationTokenSource.Token);
        ProjectionTypeName.SubscribeAsync(async typeName =>
        {
            var projectionTypes = Get().ProjectionTypes;
            if (Get().ProjectionTypes.Count < 1 || Get().ProjectionType?.Name == typeName) return;
            SetProjectionType();
            await ListProjectionsAsync();
        }, CancellationTokenSource.Token);
        QueryOptions.SubscribeAsync(async _ => await ListProjectionsAsync(), CancellationTokenSource.Token);
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
    /// Sets the <see cref="ProjectionListState.QueryOptions"/>'s <see cref="QueryOptions.Search"/>
    /// </summary>
    /// <param name="term">The <see cref="ProjectionListState.QueryOptions"/>'s <see cref="QueryOptions.Search"/></param>
    public void SetSearchTerm(string? term)
    {
        Reduce(state => state with
        {
            QueryOptions = state.QueryOptions with
            {
                Search = term
            }
        });
    }

    /// <summary>
    /// Lists available <see cref="ProjectionType"/>s
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task ListProjectionTypesAsync()
    {
        var projectionTypes = (await cloudShapesApi.ProjectionTypes.ListAsync(cancellationToken: CancellationTokenSource.Token)).Items.OrderBy(p => p.Name).ToList();
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
        var projections = await cloudShapesApi.Projections.ListAsync(Get().ProjectionType!.Name, Get().QueryOptions, CancellationTokenSource.Token);
        Reduce(state => state with
        {
            Projections = projections
        });
    }

    /// <summary>
    /// Deletes the specified projection
    /// </summary>
    /// <param name="id">The id of the projection to delete</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task DeleteProjectionAsync(string id)
    {
        await cloudShapesApi.Projections.DeleteAsync(Get().ProjectionType!.Name, id, CancellationTokenSource.Token);
        await ListProjectionsAsync();
    }

    /// <summary>
    /// Deletes selected projections
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task DeleteSelectedProjectionsAsync()
    {
        foreach (var projectionId in Get().SelectedProjections) await this.DeleteProjectionAsync(projectionId);
    }

    /// <summary>
    /// Toggles the selection of the projection with the specified id, if any, else of all projections
    /// </summary>
    /// <param name="id">The id of the projection to select</param>
    public virtual void ToggleProjectionSelection(string? id = null)
    {
        this.Reduce(state =>
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                if (state.SelectedProjections.Count > 0)
                {
                    return state with
                    {
                        SelectedProjections = []
                    };
                }
                return state with
                {
                    SelectedProjections = [.. state.Projections?.Items.Select(projection => (string)projection["_id"]) ?? []]
                };
            }
            if (state.SelectedProjections.Contains(id))
            {
                return state with
                {
                    SelectedProjections = [.. state.SelectedProjections.Where(n => n != id)]
                };
            }
            return state with
            {
                SelectedProjections = [.. state.SelectedProjections, id]
            };
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
        var navItems = new List<NavItem>()
        {
            new()
            {
                Href = $"/projections/types/new",
                IconName = IconName.PlusSquare,
                Text = "New...",
                Class = "border-bottom border-secondary-subtle"
            }
        };
        navItems.AddRange(Get().ProjectionTypes.Select(t =>
        {
            var plural = pluralize.Pluralize(t.Name);
            return new NavItem()
            {
                Href = $"/{plural.ToCamelCase()}",
                IconName = IconName.Cast,
                Text = $"{plural} ({t.Metadata.ProjectionCount})"
            };
        }));
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

    /// <summary>
    /// Handles the specified <see cref="CloudEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected async Task OnCloudEventAsync(CloudEvent e)
    {
        await ListProjectionTypesAsync();
        await ListProjectionsAsync();
    }

}