using System.Collections;
using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

public sealed class SearchEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public required string SearchTerm { get; set; }
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