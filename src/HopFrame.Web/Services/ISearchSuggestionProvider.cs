using HopFrame.Core.Config;

namespace HopFrame.Web.Services;

public interface ISearchSuggestionProvider {

    public IEnumerable<string> GenerateSearchSuggestions(TableConfig table, string searchText);

    public string CompleteSearchSuggestion(TableConfig table, string searchText, string selectedSuggestion);

}