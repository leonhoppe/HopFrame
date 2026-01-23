using HopFrame.Core.Config;

namespace HopFrame.Web.Services;

/// <summary>
/// Accessor for the advanced search suggestion feature
/// </summary>
public interface ISearchSuggestionProvider {

    /// <summary>
    /// Generates a list of search suggestions based on the current search term
    /// </summary>
    /// <param name="table">The current table for context</param>
    /// <param name="searchText">The partial input by the user</param>
    /// <returns></returns>
    public IEnumerable<string> GenerateSearchSuggestions(TableConfig table, string searchText);

    /// <summary>
    /// Generates the new search term by appending the generated text of the selected suggestion
    /// </summary>
    /// <param name="table">The current table for context</param>
    /// <param name="searchText">The partial input by the user</param>
    /// <param name="selectedSuggestion">The suggestion that was selected</param>
    /// <returns></returns>
    public string CompleteSearchSuggestion(TableConfig table, string searchText, string selectedSuggestion);

}