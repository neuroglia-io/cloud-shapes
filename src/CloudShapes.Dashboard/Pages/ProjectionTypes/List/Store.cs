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

using CloudShapes.Integration.Models;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.List;

/// <summary>
/// Represents the projection type list view's <see cref="ComponentStore{TState}"/>
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="cloudEventHub">The service used to listen to <see cref="CloudEvent"/>s produced by Cloud Shapes</param>
public class ProjectionTypeListStore(ICloudShapesApiClient cloudShapesApi, CloudEventHubClient cloudEventHub)
    : ComponentStore<ProjectionTypeListState>(new())
{

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

    /// <inheritdoc/>
    public override async Task InitializeAsync() 
    {
        await base.InitializeAsync();
        await cloudEventHub.StartAsync();
        CloudEvents = cloudEventHub.Stream();
        CloudEvents.Where(e => CloudShapes.CloudEvents.ProjectionTypes.GetTypes().Contains(e.Type)).SubscribeAsync(OnCloudEventAsync, this.CancellationTokenSource.Token);
        await ListProjectionTypesAsync();
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
    /// Lists available <see cref="ProjectionType"/>s
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task ListProjectionTypesAsync()
    {
        var projectionTypes = await cloudShapesApi.ProjectionTypes.ListAsync(cancellationToken: CancellationTokenSource.Token);
        Reduce(state => state with
        {
            ProjectionTypes = projectionTypes
        });
    }

    /// <summary>
    /// Deletes the specified <see cref="ProjectionType"/>
    /// </summary>
    /// <param name="projectionType">The <see cref="ProjectionType"/> to delete</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    public async Task DeleteProjectionTypeAsync(ProjectionType projectionType)
    {
        await cloudShapesApi.ProjectionTypes.DeleteAsync(projectionType.Name, CancellationTokenSource.Token);
        await ListProjectionTypesAsync();
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
    protected Task OnCloudEventAsync(CloudEvent e) => ListProjectionTypesAsync();

}
