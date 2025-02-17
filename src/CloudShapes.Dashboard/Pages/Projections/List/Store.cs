// Copyright © 2025-Present The Cloud Shapes Authors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using CloudShapes.Dashboard.StateManagement;

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


    bool _disposed;
    readonly BehaviorSubject<System.Reactive.Unit> _refresh = new(System.Reactive.Unit.Default);

    /// <summary>
    /// The reference to the component used to virtualize the list of projections
    /// </summary>
    public Virtualize<IDictionary<string, object>>? Virtualize { get; set; }

    #region Selectors
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
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.ProjectionId"/> changes
    /// </summary>
    public IObservable<string?> ProjectionId => this.Select(state => state.ProjectionId).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Projections"/> changes
    /// </summary>
    public IObservable<PagedResult<IDictionary<string, object>>?> Projections => this.Select(state => state.Projections).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.SelectedProjections"/> changes
    /// </summary>
    public IObservable<EquatableList<string>> SelectedProjections => this.Select(state => state.SelectedProjections).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Limit"/> changes
    /// </summary>
    public IObservable<int?> Limit => this.Select(state => state.Limit).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Skip"/> changes
    /// </summary>
    public IObservable<int?> Skip => this.Select(state => state.Skip).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Search"/> changes
    /// </summary>
    public IObservable<string?> Search => this.Select(state => state.Search).Throttle(TimeSpan.FromMilliseconds(300)).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.OrderBy"/> changes
    /// </summary>
    public IObservable<string?> OrderBy => this.Select(state => state.OrderBy).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Descending"/> changes
    /// </summary>
    public IObservable<bool?> Descending => this.Select(state => state.Descending).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Filters"/> changes
    /// </summary>
    public IObservable<EquatableDictionary<string, string>?> Filters => this.Select(state => state.Filters).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe the active projection
    /// </summary>
    public IObservable<IDictionary<string, object>?> Projection => Observable.CombineLatest(
        ProjectionId,
        Projections,
        (projectionId, projections) => projectionId == null ? null : projections?.Items.FirstOrDefault(p => (string)p["_id"] == projectionId)
    ).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe a derived <see cref="QueryOptions"/> changes
    /// </summary>
    public IObservable<QueryOptions> QueryOptions => Observable.CombineLatest(
             Limit,
             Skip,
             Search,
             OrderBy,
             Descending,
             Filters,
             (limit, skip, search, orderBy, descending, filters) => new QueryOptions
             {
                 Limit = limit,
                 Skip = skip,
                 Search = search,
                 OrderBy = orderBy,
                 Descending = descending ?? false,
                 Filters = filters?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
             }
        )
        .Throttle(TimeSpan.FromMilliseconds(100))
        .DistinctUntilChanged();
    #endregion

    #region Setters
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
    /// Sets the <see cref="ProjectionListState.ProjectionId"/>
    /// </summary>
    /// <param name="projectionId">The type of the projections to list</param>
    public void SetProjectionId(string? projectionId)
    {
        Reduce(state => state with
        {
            ProjectionId = projectionId
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.Limit"/>
    /// </summary>
    /// <param name="limit">The new value</param>
    public void SetLimit(int? limit)
    {
        Reduce(state => state with
        {
            Limit = limit
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.Skip"/>
    /// </summary>
    /// <param name="skip">The new value</param>
    public void SetSkip(int? skip)
    {
        Reduce(state => state with
        {
            Skip = skip
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.Search"/>
    /// </summary>
    /// <param name="search">The new value</param>
    public void SetSearchTerm(string? search)
    {
        Reduce(state => state with
        {
            Search = search
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.OrderBy"/>
    /// </summary>
    /// <param name="orderBy">The new value</param>
    public void SetOrderBy(string? orderBy)
    {
        Reduce(state => state with
        {
            OrderBy = orderBy
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionListState.Descending"/>
    /// </summary>
    /// <param name="descending">The new value</param>
    public void SetDescending(bool? descending)
    {
        Reduce(state => state with
        {
            Descending = descending
        });
    }

    /// <summary>
    /// Adds an item to <see cref="ProjectionListState.Filters"/>
    /// </summary>
    /// <param name="key">The key to add</param>
    /// <param name="value">The value to add</param>
    public void AddFilter(string key, string value)
    {
        var filters = Get().Filters ?? new();
        filters.Add(key, value);
        Reduce(state => state with
        {
            Filters = new(filters)
        });
    }

    /// <summary>
    /// Removes an item from <see cref="ProjectionListState.Filters"/>
    /// </summary>
    /// <param name="key">The key to remove</param>
    public void RemoveFilter(string key)
    {
        var filters = Get().Filters ?? new();
        if (!filters.ContainsKey(key)) return;
        filters.Remove(key);
        Reduce(state => state with
        {
            Filters = new(filters)
        });
    }
    #endregion

    #region Actions
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
    public async Task ListProjectionsAsync(QueryOptions queryOptions)
    {
        var projectionTypes = Get().ProjectionTypes;
        if (projectionTypes == null || projectionTypes.Count < 1) return;
        var projections = await cloudShapesApi.Projections.ListAsync(Get().ProjectionType!.Name, queryOptions, CancellationTokenSource.Token);
        Reduce(state => state with
        {
            Projections = projections
        });
        if (Virtualize != null)
        {
            await Virtualize.RefreshDataAsync();
        }
    }

    /// <summary>
    /// Deletes the specified projection
    /// </summary>
    /// <param name="id">The id of the projection to delete</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task DeleteProjectionAsync(string id)
    {
        try
        {
            await cloudShapesApi.Projections.DeleteAsync(Get().ProjectionType!.Name, id, CancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            throw; //todo: show error to end user instead
        }
        _refresh.OnNext(System.Reactive.Unit.Default);
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
                Href = $"/types/new",
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
                Href = $"/projections/{plural.ToCamelCase()}",
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
    #endregion

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await cloudEventHub.StartAsync().ConfigureAwait(false);
        CloudEvents = cloudEventHub.Stream();
        CloudEvents.Where(e => CloudShapes.CloudEvents.ProjectionTypes.GetTypes().Contains(e.Type)).SubscribeAsync(async _ => await ListProjectionTypesAsync(), CancellationTokenSource.Token);
        CloudEvents.Where(e => CloudShapes.CloudEvents.Projections.GetTypes().Contains(e.Type) && (string)((IDictionary<string, object>)e.Data!)["type"] == Get().ProjectionType?.Name).SubscribeAsync(OnCloudEventAsync, CancellationTokenSource.Token);
        Observable.CombineLatest(
            _refresh,
            ProjectionTypeName,
            QueryOptions,
            (_, typeName, queryOptions) => (typeName, queryOptions)
        )
            .Where(payload => {
                var (typeName, _) = payload;
                return Get().ProjectionTypes.Count >= 1 && Get().ProjectionType?.Name != typeName;
            })
            .Select(payload => payload.queryOptions)
            .SubscribeAsync(ListProjectionsAsync, CancellationTokenSource.Token);
        ProjectionTypeName.Subscribe(typeName =>
        {
            var projectionTypes = Get().ProjectionTypes;
            if (Get().ProjectionTypes.Count < 1 || Get().ProjectionType?.Name == typeName) return;
            SetProjectionType();
        }, CancellationTokenSource.Token);
        await ListProjectionTypesAsync();
        SetLoading(false);
    }

    /// <summary>
    /// Handles the specified <see cref="CloudEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected async Task OnCloudEventAsync(CloudEvent e)
    {
        await ListProjectionTypesAsync();
        _refresh.OnNext(System.Reactive.Unit.Default);
    }

    /// <summary>
    /// Disposes of the store
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the dispose of the store</param>
    protected override void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._refresh.OnCompleted();
            }
            this._disposed = true;
        }
    }

}