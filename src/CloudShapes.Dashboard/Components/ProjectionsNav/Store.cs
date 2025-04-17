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

using BlazorBootstrap;
using CloudShapes.Dashboard.Pages.Projections.List;

namespace CloudShapes.Dashboard.Components.ProjectionsNavStateManagement;

/// <summary>
/// Represents the <see cref="ComponentStore{TState}" /> of a <see cref="ProjectionsNav"/>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="pluralize">The service used to pluralize words</param>
/// </summary>
public class ProjectionsNavStore(ICloudShapesApiClient cloudShapesApi, IPluralize pluralize)
    : ComponentStore<ProjectionsNavState>(new())
{

    /// <summary>
    /// The <see cref="Sidebar2"/> reference used in the view
    /// </summary>
    public Sidebar2? Sidebar { get; set; }

    #region Selectors
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.Loading"/> changes
    /// </summary>
    public IObservable<bool> Loading => this.Select(state => state.Loading).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionListState.ProjectionTypes"/> changes
    /// </summary>
    public IObservable<EquatableList<ProjectionType>> ProjectionTypes => this.Select(state => state.ProjectionTypes ?? []).DistinctUntilChanged();
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
            ProjectionTypes = new(projectionTypes),
            Loading = false
        });
    }

    /// <summary>
    /// Provides data to the view's sidebar
    /// </summary>
    /// <param name="request">The <see cref="SidebarDataProviderRequest"/> to handle</param>
    /// <returns>A new <see cref="SidebarDataProviderResult"/></returns>
    public Task<Sidebar2DataProviderResult> ProvideSidebarDataAsync(Sidebar2DataProviderRequest request)
    {
        var navItems = new List<NavItem>()
        {
            new()
            {
                Id = "new-type",
                Href = $"/types/new",
                IconName = IconName.PlusSquare,
                Text = "New...",
                Class = "border-bottom border-secondary-subtle"
            }
        };
        navItems.AddRange(Get().ProjectionTypes.SelectMany(t =>
        {
            var plural = pluralize.Pluralize(t.Name);
            return new List<NavItem>([
                new NavItem()
                {
                    Id = $"{plural}-menu",
                    Text = $"{plural} ({t.Metadata.ProjectionCount})"
                },
                new NavItem()
                {
                    Id = $"{plural}-list",
                    ParentId = $"{plural}-menu",
                    Href = $"/projections/{plural.ToCamelCase()}",
                    IconName = IconName.List,
                    Text = $"List ({t.Metadata.ProjectionCount})"
                },
                new NavItem()
                {
                    Id = $"{plural}-list",
                    ParentId = $"{plural}-menu",
                    Href = $"/types/schema/{t.Name}",
                    IconName = IconName.InfoCircle,
                    Text = $"Schema"
                },
                new NavItem()
                {
                    Id = $"{plural}-list",
                    ParentId = $"{plural}-menu",
                    Href = $"/types/triggers/{t.Name}",
                    IconName = IconName.LightningChargeFill,
                    Text = $"Triggers"
                },
                new NavItem()
                {
                    Id = $"{plural}-list",
                    ParentId = $"{plural}-menu",
                    Href = $"/types/relationships/{t.Name}",
                    IconName = IconName.Link,
                    Text = $"Relationships"
                },
                new NavItem()
                {
                    Id = $"{plural}-list",
                    ParentId = $"{plural}-menu",
                    Href = $"/types/indexes/{t.Name}",
                    IconName = IconName.ListOl,
                    Text = $"Indexes"
                },
            ]);
        }));
        return Task.FromResult(request.ApplyTo(navItems));
    }
    #endregion

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Reduce(state => state with
        {
            Loading = true
        });
        await Task.Delay(1);
        await ListProjectionTypesAsync();
        if (Sidebar is not null)
        {
            await Sidebar.RefreshDataAsync();
        }
    }

}