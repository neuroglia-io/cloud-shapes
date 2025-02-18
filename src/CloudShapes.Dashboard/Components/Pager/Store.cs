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

namespace CloudShapes.Dashboard.Components.PagerStateManagement;

/// <summary>
/// Represents the <see cref="ComponentStore{TState}" /> of a <see cref="Pager"/>
/// </summary>
public class PagerStore()
    : ComponentStore<PagerState>(new())
{

    #region Selectors
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="PagerState.TotalLength"/> changes
    /// </summary>
    public IObservable<long> TotalLength => this.Select(state => state.TotalLength).DistinctUntilChanged();
    
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="PagerState.PageIndex"/> changes
    /// </summary>
    public IObservable<int> PageIndex => this.Select(state => state.PageIndex).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="PagerState.PreviousPageIndex"/> changes
    /// </summary>
    public IObservable<int> PreviousPageIndex => this.Select(state => state.PreviousPageIndex).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="PagerState.PageSize"/> changes
    /// </summary>
    public IObservable<int> PageSize => this.Select(state => state.PageSize).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="PagerState.PreviousPageSize"/> changes
    /// </summary>
    public IObservable<int> PreviousPageSize => this.Select(state => state.PreviousPageSize).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe the number of pages
    /// </summary>
    public IObservable<int> PageCount => Observable.CombineLatest(
        TotalLength,
        PageSize,
        (totalLength, pageSize) => (int)((totalLength + pageSize - 1) / pageSize) /*rounds up*/
    ).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe if the current page is the first page
    /// </summary>
    public IObservable<bool> IsFirstPage => Observable.CombineLatest(
        PageCount,
        PageIndex,
        (pageCount, pageIndex) => pageCount == 0 || pageIndex == 0
    ).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe if the current page is the last page
    /// </summary>
    public IObservable<bool> IsLastPage => Observable.CombineLatest(
        PageCount,
        PageIndex,
        (pageCount, pageIndex) => pageCount == 0 || (pageIndex + 1) == pageCount
    ).DistinctUntilChanged();
    #endregion

    #region Setters
    /// <summary>
    /// Sets the state's <see cref="PagerState.TotalLength"/>
    /// </summary>
    /// <param name="totalLength">The new value</param>
    public void SetTotalLength(long totalLength)
    {
        Reduce(state => state with {
            TotalLength = totalLength
        });
    }

    /// <summary>
    /// Sets the state's <see cref="PagerState.PageIndex"/>
    /// </summary>
    /// <param name="pageIndex">The new value</param>
    public void SetPageIndex(int pageIndex)
    {
        var previousPageIndex = this.Get(state => state.PageIndex);
        Reduce(state => state with
        {
            PageIndex = pageIndex,
            PreviousPageIndex = previousPageIndex
        });
    }

    /// <summary>
    /// Sets the state's <see cref="PagerState.PageSize"/>
    /// </summary>
    /// <param name="pageSize">The new value</param>
    public void SetPageSize(int pageSize)
    {
        var previousPageSize = this.Get(state => state.PageSize);
        Reduce(state => state with
        {
            PageSize = pageSize,
            PreviousPageSize = previousPageSize
        });
    }
    #endregion

}