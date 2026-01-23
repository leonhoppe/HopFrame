using System.Collections;
using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// Raised before the search results are loaded
/// </summary>
/// <param name="sender"></param>
public sealed class SearchEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    /// <summary>
    /// The search term the user entered
    /// </summary>
    public required string SearchTerm { get; set; }
    
    /// <summary>
    /// The page the user is currently on
    /// </summary>
    public required int CurrentPage { get; init; }
    
    internal IEnumerable<object>? SearchResult { get; set; }
    internal int TotalPages { get; set; }

    /// <summary>
    /// Sets the new search result that is being displayed<br />
    /// The event needs to be canceled in order for the custom search results to appear
    /// </summary>
    /// <param name="result">The current page of search results</param>
    /// <param name="totalPages">The total pages of search results</param>
    public void SetSearchResult(IEnumerable result, int totalPages) {
        SearchResult = result.OfType<object>();
        TotalPages = totalPages;
    }
}