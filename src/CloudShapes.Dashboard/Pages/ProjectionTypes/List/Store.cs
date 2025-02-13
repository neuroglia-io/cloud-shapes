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

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.List;

/// <summary>
/// Represents the projection type list view's <see cref="ComponentStore{TState}"/>
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="cloudEventHub">The service used to listen to <see cref="CloudEvent"/>s produced by Cloud Shapes</param>
public class ProjectionTypeListStore(ICloudShapesApiClient cloudShapesApi, CloudEventHubClient cloudEventHub)
    : ComponentStore<ProjectionTypeListState>(new())
{

    bool _disposed;
    readonly BehaviorSubject<System.Reactive.Unit> _refresh = new(System.Reactive.Unit.Default);

    /// <summary>
    /// The reference to the component used to virtualize the list of projections
    /// </summary>
    public Virtualize<ProjectionType>? Virtualize { get; set; }

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe the <see cref="CloudEvent"/>s produced by Cloud Shapes
    /// </summary>
    protected IObservable<CloudEvent> CloudEvents { get; private set; } = null!;

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.ProjectionTypes"/> changes
    /// </summary>
    public IObservable<PagedResult<ProjectionType>?> ProjectionTypes => this.Select(state => state.ProjectionTypes).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.SelectedProjectionTypes"/> changes
    /// </summary>
    public IObservable<EquatableList<string>> SelectedProjectionTypes => this.Select(state => state.SelectedProjectionTypes).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Limit"/> changes
    /// </summary>
    public IObservable<int?> Limit => this.Select(state => state.Limit).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Skip"/> changes
    /// </summary>
    public IObservable<int?> Skip => this.Select(state => state.Skip).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Search"/> changes
    /// </summary>
    public IObservable<string?> Search => this.Select(state => state.Search).Throttle(TimeSpan.FromMilliseconds(300)).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.OrderBy"/> changes
    /// </summary>
    public IObservable<string?> OrderBy => this.Select(state => state.OrderBy).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Descending"/> changes
    /// </summary>
    public IObservable<bool> Descending => this.Select(state => state.Descending).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Filters"/> changes
    /// </summary>
    public IObservable<EquatableDictionary<string, string>?> Filters => this.Select(state => state.Filters).DistinctUntilChanged();

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
                 Descending = descending,
                 Filters = filters?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
             }
        )
        .Throttle(TimeSpan.FromMilliseconds(100))
        .DistinctUntilChanged();

    /// <inheritdoc/>
    public override async Task InitializeAsync() 
    {
        await base.InitializeAsync();
        await cloudEventHub.StartAsync();
        CloudEvents = cloudEventHub.Stream();
        CloudEvents.Where(e => CloudShapes.CloudEvents.ProjectionTypes.GetTypes().Contains(e.Type)).Subscribe(OnCloudEvent, this.CancellationTokenSource.Token);
        Observable.CombineLatest(
            _refresh,
            QueryOptions,
            (_, queryOptions) => queryOptions
        )
            .SubscribeAsync(async (queryOptions) => await ListProjectionTypesAsync(queryOptions), CancellationTokenSource.Token);
        SetLoading(false);
    }

    /// <summary>
    /// Sets the <see cref="ProjectionTypeListState.Loading"/>
    /// </summary>
    /// <param name="loading">The new loading state</param>
    public void SetLoading(bool loading)
    {
        this.Reduce(state => state with
        {
            Loading = loading
        });
    }

    /// <summary>
    /// Sets the <see cref="ProjectionTypeListState.Limit"/>
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
    /// Sets the <see cref="ProjectionTypeListState.Skip"/>
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
    /// Sets the <see cref="ProjectionTypeListState.Search"/>
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
    /// Sets the <see cref="ProjectionTypeListState.OrderBy"/>
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
    /// Sets the <see cref="ProjectionTypeListState.Descending"/>
    /// </summary>
    /// <param name="descending">The new value</param>
    public void SetDescending(bool descending)
    {
        Reduce(state => state with
        {
            Descending = descending
        });
    }

    /// <summary>
    /// Adds an item to <see cref="ProjectionTypeListState.Filters"/>
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
    /// Removes an item from <see cref="ProjectionTypeListState.Filters"/>
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

    /// <summary>
    /// Toggles the selection of the projection type with the specified name, if any, else of all projection types
    /// </summary>
    /// <param name="name">The name of the projection type to select</param>
    public virtual void ToggleProjectionTypeSelection(string? name = null)
    {
        this.Reduce(state =>
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                if (state.SelectedProjectionTypes.Count > 0)
                {
                    return state with
                    {
                        SelectedProjectionTypes = []
                    };
                }
                return state with
                {
                    SelectedProjectionTypes = [.. state.ProjectionTypes?.Items.Select(projectionType => projectionType.Name) ?? []]
                };
            }
            if (state.SelectedProjectionTypes.Contains(name))
            {
                return state with
                {
                    SelectedProjectionTypes = [.. state.SelectedProjectionTypes.Where(n => n != name)]
                };
            }
            return state with
            {
                SelectedProjectionTypes = [.. state.SelectedProjectionTypes, name]
            };
        });
    }


    /// <summary>
    /// Lists available <see cref="ProjectionType"/>s
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task ListProjectionTypesAsync(QueryOptions queryOptions)
    {
        var projectionTypes = await cloudShapesApi.ProjectionTypes.ListAsync(queryOptions, CancellationTokenSource.Token);
        Reduce(state => state with
        {
            ProjectionTypes = projectionTypes
        });
        if (Virtualize != null)
        {
            await Virtualize.RefreshDataAsync();
        }
    }

    /// <summary>
    /// Deletes the specified <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="typeName">The name of <see cref="ProjectionType"/> to delete</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task DeleteProjectionTypeAsync(string typeName)
    {
        await cloudShapesApi.ProjectionTypes.DeleteAsync(typeName, CancellationTokenSource.Token);
        _refresh.OnNext(System.Reactive.Unit.Default);
    }

    /// <summary>
    /// Deletes selected <see cref="ProjectionType"/>s
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task DeleteSelectedProjectionTypesAsync()
    {
        foreach (var typeName in Get().SelectedProjectionTypes) await this.DeleteProjectionTypeAsync(typeName);
    }

    /// <summary>
    /// Provides items to the <see cref="Virtualize{TItem}"/> component
    /// </summary>
    /// <param name="request">The <see cref="ItemsProviderRequest"/> to execute</param>
    /// <returns>The resulting <see cref="ItemsProviderResult{TResult}"/></returns>
    public ValueTask<ItemsProviderResult<ProjectionType>> ProvideProjectionTypesAsync(ItemsProviderRequest request)
    {
        var projectionTypes = Get().ProjectionTypes;
        if (projectionTypes == null) return ValueTask.FromResult(new ItemsProviderResult<ProjectionType>());
        else return ValueTask.FromResult(new ItemsProviderResult<ProjectionType>(projectionTypes.Items.OrderBy(p => p.Name), (int)projectionTypes.TotalCount));
    }

    /// <summary>
    /// Handles the specified <see cref="CloudEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to handle</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected void OnCloudEvent(CloudEvent e) => _refresh.OnNext(System.Reactive.Unit.Default);

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
