using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.List;

/// <summary>
/// Represents the projection type list view's <see cref="ComponentStore{TState}"/>
/// </summary>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
public class ProjectionTypeListStore(ICloudShapesApiClient cloudShapesApi)
    : ComponentStore<ProjectionTypeListState>(new())
{

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeListState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <inheritdoc/>
    public override Task InitializeAsync() => base.InitializeAsync();

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
    /// Provides items to the <see cref="Virtualize{TItem}"/> component
    /// </summary>
    /// <param name="request">The <see cref="ItemsProviderRequest"/> to execute</param>
    /// <returns>The resulting <see cref="ItemsProviderResult{TResult}"/></returns>
    public async ValueTask<ItemsProviderResult<ProjectionType>> ProvideProjectionTypesAsync(ItemsProviderRequest request)
    {
        this.SetLoading(true);
        var fetchedProjectionTypes = await cloudShapesApi.ProjectionTypes.ListAsync();
        this.SetLoading(false);
        return new ItemsProviderResult<ProjectionType>(fetchedProjectionTypes.Items, (int)fetchedProjectionTypes.TotalCount);
    }

}
